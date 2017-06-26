namespace DistributedStorage.Networking.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;

    public sealed class MagicGuidProtocolInitializer<T> : IProtocolInitializer<T>
    {
        private readonly IReadOnlyList<Tuple<Guid, Func<T, IHandler<byte[], byte[]>>>> _handlerFactoryTuples;

        public MagicGuidProtocolInitializer(Func<Type, IConverter<byte[], object>> deserializerLookup, Func<Type, IConverter<object, byte[]>> serializerLookup)
        {
            var deserializers = new Dictionary<Type, IConverter<byte[], object>>();
            var serializers = new Dictionary<Type, IConverter<object, byte[]>>();
            var handlerFactories = new List<Tuple<Guid, Func<T, IHandler<byte[], byte[]>>>>();
            _handlerFactoryTuples = handlerFactories;

            foreach (var method in typeof(T).GetMethods())
            {
                var attribute = method.GetCustomAttribute<GuidAttribute>();
                if (attribute == null)
                    continue;
                var guid = attribute.Guid;

                var returnType = method.ReturnType;
                if (!serializers.TryGetValue(returnType, out var returnSerializer))
                    returnSerializer = serializers[returnType] = serializerLookup(returnType);
                var parameterDeserializers = method.GetParameters().Select(x => x.ParameterType).Select(parameterType =>
                    {
                        if (!deserializers.TryGetValue(parameterType, out var parameterDeserializer))
                            parameterDeserializer = deserializers[parameterType] = deserializerLookup(parameterType);
                        return parameterDeserializer;
                    })
                    .ToArray();

                IHandler<byte[], byte[]> CreateHandlerFor(T obj)
                {
                    var handler = new Handler<byte[], byte[]>(input =>
                    {
                        using (var stream = new MemoryStream(input))
                        {
                            var parameters = parameterDeserializers.Select(deserializer =>
                            {
                                if (!stream.TryRead(out byte[] serializedParameter))
                                    throw new Exception();
                                if (!deserializer.TryConvert(serializedParameter, out var deserializedParameter))
                                    throw new Exception();
                                return deserializedParameter;
                            }).ToArray();

                            var result = method.Invoke(obj, parameters);

                            if (!returnSerializer.TryConvert(result, out var serializedResult))
                                throw new Exception();
                            return serializedResult;
                        }
                    });
                    return handler;
                }

                handlerFactories.Add(Tuple.Create(guid, (Func<T, IHandler<byte[], byte[]>>)CreateHandlerFor));
            }
        }

        public bool TrySetup(IProtocol protocol, T with, out IDisposable tearDown)
        {
            var registeredGuids = new List<Guid>();
            tearDown = new Disposable(() =>
            {
                foreach (var registeredGuid in registeredGuids)
                    protocol.TryUnregister(registeredGuid.ToString());
            });

            foreach (var tuple in _handlerFactoryTuples)
            {
                var guid = tuple.Item1;
                var handlerFactory = tuple.Item2;
                var handler = handlerFactory(with);
                if (!protocol.TryRegister(guid.ToString(), handler))
                    return false;
                registeredGuids.Add(guid);
            }
            return true;
        }
    }
}

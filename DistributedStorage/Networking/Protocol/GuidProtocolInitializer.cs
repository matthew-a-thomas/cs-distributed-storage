namespace DistributedStorage.Networking.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Common;

    /// <summary>
    /// An <see cref="IProtocolInitializer{T}"/> that registers <see cref="IHandler{TParameter, TResult}"/> factories for <see cref="Guid"/>s
    /// </summary>
    public sealed class GuidProtocolInitializer<T> : IProtocolInitializer<T>
    {
        /// <summary>
        /// The list of handler factories to use
        /// </summary>
        private readonly IReadOnlyList<Tuple<Guid, Func<T, IHandler<byte[], byte[]>>>> _handlerFactoryTuples;

        /// <summary>
        /// Creates a new <see cref="GuidProtocolInitializer{T}"/>, which registers <see cref="IHandler{TParameter, TResult}"/> factories for <see cref="Guid"/>s
        /// </summary>
        public GuidProtocolInitializer(IReadOnlyList<Tuple<Guid, Func<T, IHandler<byte[], byte[]>>>> handlerFactoryTuples)
        {
            _handlerFactoryTuples = handlerFactoryTuples;
        }
        
        /// <summary>
        /// Tries to register an <see cref="IHandler{TParameter, TResult}"/> for each <see cref="Guid"/> in the list that was passed into this <see cref="GuidProtocolInitializer{T}"/>'s constructor
        /// </summary>
        public bool TrySetup(IProtocol protocol, T with, out IDisposable tearDown)
        {
            // Start a list of registered GUIDs
            var registeredGuids = new List<Guid>();

            // Set up the tear down routine, which will try to unregister all registered GUIDs
            tearDown = new Disposable(() =>
            {
                foreach (var registeredGuid in registeredGuids)
                    protocol.TryUnregister(registeredGuid.ToString());
            });

            // Run through each handler factory, and register each generated handler with the corresponding GUID
            foreach (var tuple in _handlerFactoryTuples)
            {
                var guid = tuple.Item1;
                var handlerFactory = tuple.Item2;
                var handler = handlerFactory(with);
                if (!protocol.TryRegister(guid.ToString(), handler))
                    return false;
                registeredGuids.Add(guid); // We successfully registered this GUID with the protocol, so add it to the list of registered GUIDs
            }

            return true;
        }
    }

    public static class GuidProtocolInitializer
    {
        /// <summary>
        /// Creates a new <see cref="GuidProtocolInitializer{T}"/> for the given type <typeparamref name="T"/>, using the given <paramref name="deserializerLookup"/> and <paramref name="serializerLookup"/> to create <see cref="IHandler{TParameter, TResult}"/>s for all methods in the type <typeparamref name="T"/> that are decorated with the <see cref="GuidAttribute"/>
        /// </summary>
        public static GuidProtocolInitializer<T> Create<T>(Func<Type, IConverter<byte[], object>> deserializerLookup, Func<Type, IConverter<object, byte[]>> serializerLookup)
        {
            var deserializers = new Dictionary<Type, IConverter<byte[], object>>();
            var serializers = new Dictionary<Type, IConverter<object, byte[]>>();
            var handlerFactoryTuples = new List<Tuple<Guid, Func<T, IHandler<byte[], byte[]>>>>();

            // Loop through all methods in type T
            foreach (var method in typeof(T).GetMethods())
            {
                // Skip any methods which don't have a GuidAttribute
                var attribute = method.GetCustomAttribute<GuidAttribute>();
                if (attribute == null)
                    continue;

                // Grab the GUID from the GuidAttribute
                var guid = attribute.Guid;

                // Create a serializer for the return type
                var returnType = method.ReturnType;
                if (!serializers.TryGetValue(returnType, out var returnSerializer))
                    returnSerializer = serializers[returnType] = serializerLookup(returnType);

                // Create parameter deserializers
                var parameterDeserializers = method.GetParameters().Select(x => x.ParameterType).Select(parameterType =>
                    {
                        if (!deserializers.TryGetValue(parameterType, out var parameterDeserializer))
                            parameterDeserializer = deserializers[parameterType] = deserializerLookup(parameterType);
                        return parameterDeserializer;
                    })
                    .ToArray();

                // Set up a handler factory for this method and a given object of the type T
                IHandler<byte[], byte[]> CreateHandlerFor(T obj)
                {
                    // Create an IHandler that will take a byte array as input,
                    // transform it into the series of parameters required for this method,
                    // pass those parameters through this method (on the given object "obj"),
                    // then serialize the return value into a byte array which is returned
                    var handler = new Handler<byte[], byte[]>(input =>
                    {
                        using (var stream = new MemoryStream(input))
                        {
                            // Try deserializing the given input into all the parameters necessary for invoking this method
                            var parameters = parameterDeserializers.Select(deserializer =>
                            {
                                // Try to read this parameter's bytes from the input
                                // ReSharper disable once AccessToDisposedClosure
                                if (!stream.TryRead(out byte[] serializedParameter))
                                    throw new Exception("Couldn't read a chunk of bytes out of the given input so that a parameter could be deserialized");
                                // Try to deserialize this chunk into a parameter value
                                if (!deserializer.TryConvert(serializedParameter, out var deserializedParameter))
                                    throw new Exception("Couldn't deserialize this chunk of bytes out as a parameter using this deserializer");
                                // Return the deserialized parameter
                                return deserializedParameter;
                            }).ToArray();

                            // Invoke the method with the deserialized parameters, and grab the result
                            var result = method.Invoke(obj, parameters);

                            // Try to serialize the method's return value
                            if (!returnSerializer.TryConvert(result, out var serializedResult))
                                throw new Exception("Couldn't serialize the return value of this method using this serializer");

                            // Return the serialized return value
                            return serializedResult;
                        }
                    });
                    return handler;
                }

                // Add this handler factory to the list of them, and pair it with this method's corresponding GUID
                handlerFactoryTuples.Add(Tuple.Create(guid, (Func<T, IHandler<byte[], byte[]>>)CreateHandlerFor));
            }

            // Return a new protocol initializer that uses the above list of handler factories
            return new GuidProtocolInitializer<T>(handlerFactoryTuples);
        }
    }
}

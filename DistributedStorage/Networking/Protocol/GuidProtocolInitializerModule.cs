namespace DistributedStorage.Networking.Protocol
{
    using System;
    using Autofac;
    using Common;

    /// <summary>
    /// Using this module registers an <see cref="IProtocolInitializer{T}"/>, implemented by <see cref="GuidProtocolInitializer{T}"/>
    /// </summary>
    public sealed class GuidProtocolInitializerModule<T> : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                Type MakeConverterType(Type from, Type to)
                {
                    var baseType = typeof(IConverter<,>);
                    var converterType = baseType.MakeGenericType(from, to);
                    return converterType;
                }

                IConverter<byte[], object> DeserializerLookup(Type type)
                {
                    var converterType = MakeConverterType(typeof(byte[]), type);
                    if (!c.TryResolve(converterType, out var instance))
                        throw new Exception($"Failed to find an {converterType.Name}");
                    if (!(instance is IConverter<byte[], object> deserializer))
                        throw new Exception("Found a deserializer, but it isn't the correct type");
                    return deserializer;
                }

                IConverter<object, byte[]> SerializerLookup(Type type)
                {
                    var converterType = MakeConverterType(type, typeof(byte[]));
                    if (!c.TryResolve(converterType, out var instance))
                        throw new Exception($"Failed to find an {converterType.Name}");
                    if (!(instance is IConverter<object, byte[]> deserializer))
                        throw new Exception("Found a serializer, but it isn't the correct type");
                    return deserializer;
                }

                if (!GuidProtocolInitializer.TryCreate<T>(DeserializerLookup, SerializerLookup, out var initializer))
                    throw new Exception($"Unable to create a new {nameof(GuidProtocolInitializer)}");

                return (IProtocolInitializer<T>) initializer;
            }).SingleInstance();
        }
    }
}

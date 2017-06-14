namespace DistributedStorage.Networking
{
    using Common;

    public static class SerializerExtensions
    {
        /// <summary>
        /// Returns a pair of <see cref="IConverter{TFrom, TTo}"/>s that use the given <see cref="ISerializer{T}"/>
        /// </summary>
        public static (IConverter<T, byte[]> toByteArray, IConverter<byte[], T> fromByteArray) ToConverters<T>(this ISerializer<T> serializer)
        {
            bool TryConvertFromBytes(byte[] bytes, out T thing) => serializer.TryDeserialize(bytes, out thing);
            bool TryConvertToBytes(T thing, out byte[] bytes)
            {
                bytes = serializer.Serialize(thing);
                return true;
            }
            return (
                new Converter<T, byte[]>(TryConvertToBytes),
                new Converter<byte[], T>(TryConvertFromBytes)
            );
        }
    }
}

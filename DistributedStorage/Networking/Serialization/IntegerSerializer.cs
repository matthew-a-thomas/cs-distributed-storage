namespace DistributedStorage.Networking.Serialization
{
    using Common;
    using System.IO;

    public sealed class IntegerSerializer : ISerializer<int>
    {
        public byte[] Serialize(int thing)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(thing);
                return stream.ToArray();
            }
        }

        public bool TryDeserialize(byte[] bytes, out int thing)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return stream.TryRead(out thing);
            }
        }
    }
}

namespace DistributedStorage.Networking.Serialization
{
    using System.IO;
    using Encoding;

    public sealed class SliceSerializer : ISerializer<Slice>
    {
        public byte[] Serialize(Slice thing)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(thing);
                return stream.ToArray();
            }
        }

        public bool TryDeserialize(byte[] bytes, out Slice thing)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return stream.TryRead(out thing);
            }
        }
    }
}

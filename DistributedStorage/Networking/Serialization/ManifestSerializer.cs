namespace DistributedStorage.Networking.Serialization
{
    using System.IO;
    using Encoding;

    public sealed class ManifestSerializer : ISerializer<Manifest>
    {
        public byte[] Serialize(Manifest thing)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(thing);
                return stream.ToArray();
            }
        }

        public bool TryDeserialize(byte[] bytes, out Manifest thing)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return stream.TryRead(out thing);
            }
        }
    }
}

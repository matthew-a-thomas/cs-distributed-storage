namespace DistributedStorage.Serialization
{
    using System.IO;
    using System.Text;
    using Common;

    /// <summary>
    /// Extension methods for a <see cref="Manifest"/> in the context of serialization
    /// </summary>
    public static class ManifestExtensions
    {
        /// <summary>
        /// Deserializes a <see cref="Manifest"/> from the given <paramref name="stream"/>
        /// </summary>
        public static Manifest GetManifest(this Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var hashLength = reader.ReadByte();
                var id = new Hash(reader.ReadBytes(hashLength));
                var length = reader.ReadInt32();
                var sliceHashes = new Hash[reader.ReadInt32()];
                for (var i = 0; i < sliceHashes.Length; ++i)
                {
                    sliceHashes[i] = new Hash(reader.ReadBytes(hashLength));
                }
                return new Manifest
                {
                    Id = id,
                    Length = length,
                    SliceHashes = sliceHashes
                };
            }
        }

        /// <summary>
        /// Serializes this <see cref="Manifest"/> into the given <paramref name="stream"/>
        /// </summary>
        public static void SerializeTo(this Manifest manifest, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write((byte) manifest.Id.HashCode.Length);
                writer.Write(manifest.Id.HashCode);
                writer.Write(manifest.Length);
                writer.Write(manifest.SliceHashes.Length);
                foreach (var sliceHash in manifest.SliceHashes)
                {
                    writer.Write(sliceHash.HashCode);
                }
            }
        }
    }
}

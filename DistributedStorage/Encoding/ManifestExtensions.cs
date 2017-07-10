namespace DistributedStorage.Encoding
{
    using System.IO;
    using Common;

    /// <summary>
    /// Extension methods for a <see cref="Manifest"/> in the context of serialization
    /// </summary>
    public static class ManifestExtensions
    {
        public static bool TryRead(this Stream stream, out Manifest manifest)
        {
            manifest = null;
            if (!stream.TryRead(out Hash id))
                return false;
            if (!stream.TryRead(out int length))
                return false;
            if (!stream.TryRead(out int numSliceHashes))
                return false;
            var sliceHashes = new Hash[numSliceHashes];
            for (var i = 0; i < numSliceHashes; ++i)
            {
                if (!stream.TryRead(out Hash sliceHash))
                    return false;
                sliceHashes[i] = sliceHash;
            }

            manifest = new Manifest
            {
                Id = id,
                Length = length,
                SliceHashes = sliceHashes
            };
            return true;
        }

        /// <summary>
        /// Serializes this <see cref="Manifest"/> into the given <paramref name="stream"/>
        /// </summary>
        public static void Write(this Stream stream, Manifest manifest)
        {
            stream.Write(manifest.Id);
            stream.Write(manifest.Length);
            stream.Write(manifest.SliceHashes.Length);
            foreach (var sliceHash in manifest.SliceHashes)
                stream.Write(sliceHash);
        }
    }
}

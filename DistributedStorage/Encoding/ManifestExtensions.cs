namespace DistributedStorage.Encoding
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Common;

    /// <summary>
    /// Extension methods for a <see cref="Manifest"/> in the context of serialization
    /// </summary>
    public static class ManifestExtensions
    {
        /// <summary>
        /// Deserializes a <see cref="Manifest"/> from the given <paramref name="stream"/>
        /// </summary>
        public static bool TryReadManifest(this Stream stream, TimeSpan timeout, out Manifest manifest)
        {
            manifest = null;
            var start = Stopwatch.StartNew();
            if (!stream.TryReadHash(timeout, out var id))
                return false;
            if (!stream.TryBlockingRead7BitEncodedInt(timeout - start.Elapsed, out var length))
                return false;
            if (!stream.TryBlockingRead7BitEncodedInt(timeout - start.Elapsed, out var numSliceHashes))
                return false;
            var sliceHashes = new Hash[numSliceHashes];
            for (var i = 0; i < numSliceHashes; ++i)
            {
                if (!stream.TryReadHash(timeout - start.Elapsed, out var sliceHash))
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
        public static void WriteManifest(this Stream stream, Manifest manifest)
        {
            stream.WriteHash(manifest.Id);
            stream.Write7BitEncodedInt(manifest.Length);
            stream.Write7BitEncodedInt(manifest.SliceHashes.Length);
            foreach (var sliceHash in manifest.SliceHashes)
                stream.WriteHash(sliceHash);
        }
    }
}

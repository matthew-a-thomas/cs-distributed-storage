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
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out Manifest manifest)
        {
            manifest = null;
            var start = Stopwatch.StartNew();
            if (!HashExtensions.TryBlockingRead(stream, timeout, out var id))
                return false;
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out int length))
                return false;
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out int numSliceHashes))
                return false;
            var sliceHashes = new Hash[numSliceHashes];
            for (var i = 0; i < numSliceHashes; ++i)
            {
                if (!HashExtensions.TryBlockingRead(stream, timeout - start.Elapsed, out var sliceHash))
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

        public static bool TryImmediateRead(this Stream stream, out Manifest manifest)
        {
            manifest = null;
            if (!stream.TryImmediateRead(out Hash id))
                return false;
            if (!stream.TryImmediateRead(out int length))
                return false;
            if (!stream.TryImmediateRead(out int numSliceHashes))
                return false;
            var sliceHashes = new Hash[numSliceHashes];
            for (var i = 0; i < numSliceHashes; ++i)
            {
                if (!stream.TryImmediateRead(out Hash sliceHash))
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

namespace DistributedStorage.Storage
{
    using System;
    using System.Diagnostics;
    using Encoding;
    using System.IO;
    using Common;
    using Networking.Security;

    public static class NodeExtensions
    {
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out Node node)
        {
            node = null;
            var start = Stopwatch.StartNew();
            if (!ManifestExtensions.TryBlockingRead(stream, timeout, out var manifest))
                return false;
            if (!RsaParametersExtensions.TryBlockingRead(stream, timeout - start.Elapsed, out var key))
                return false;
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out int numSlices))
                return false;
            var slices = new Slice[numSlices];
            for (var i = 0; i < numSlices; ++i)
            {
                if (!SliceExtensions.TryBlockingRead(stream, timeout - start.Elapsed, out var slice))
                    return false;
                slices[i] = slice;
            }

            node = new Node
            {
                Key = key,
                Manifest = manifest,
                Slices = slices
            };
            return true;
        }

        public static void Write(this Stream stream, Node node)
        {
            stream.Write(node.Manifest);
            stream.Write(node.Key);
            stream.Write(node.Slices.Count);
            foreach (var slice in node.Slices)
                stream.Write(slice);
        }
    }
}

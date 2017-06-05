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
            if (!stream.TryReadManifest(timeout, out var manifest))
                return false;
            if (!stream.TryReadRsaKey(timeout - start.Elapsed, out var key))
                return false;
            if (!stream.TryBlockingRead7BitEncodedInt(timeout - start.Elapsed, out var numSlices))
                return false;
            var slices = new Slice[numSlices];
            for (var i = 0; i < numSlices; ++i)
            {
                if (!stream.TryReadSlice(timeout - start.Elapsed, out var slice))
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
            stream.WriteManifest(node.Manifest);
            stream.WritePublicKey(node.Key);
            stream.Write7BitEncodedInt(node.Slices.Count);
            foreach (var slice in node.Slices)
                stream.WriteSlice(slice);
        }
    }
}

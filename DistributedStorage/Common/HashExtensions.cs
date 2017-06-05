namespace DistributedStorage.Common
{
    using System;
    using System.IO;

    public static class HashExtensions
    {
        public static bool TryReadHash(this Stream stream, TimeSpan timeout, out Hash hash)
        {
            hash = null;
            if (!stream.TryBlockingReadChunk(timeout, out var hashCode))
                return false;
            if (hashCode.Length > Hash.NumBytes)
                return false;
            hash = new Hash(hashCode);
            return true;
        }

        public static void WriteHash(this Stream stream, Hash hash) => stream.WriteChunk(hash.HashCode);
    }
}

namespace DistributedStorage.Common
{
    using System;
    using System.IO;

    public static class HashExtensions
    {
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out Hash hash)
        {
            hash = null;
            if (!stream.TryBlockingRead(timeout, out byte[] hashCode))
                return false;
            if (hashCode.Length > Hash.NumBytes)
                return false;
            hash = new Hash(hashCode);
            return true;
        }

        public static bool TryImmediateRead(this Stream stream, out Hash hash)
        {
            hash = null;
            if (!stream.TryImmediateRead(out byte[] hashCode))
                return false;
            if (hashCode.Length > Hash.NumBytes)
                return false;
            hash = new Hash(hashCode);
            return true;
        }

        public static void Write(this Stream stream, Hash hash) => stream.Write(hash.HashCode);
    }
}

namespace DistributedStorage.Common
{
    using System.IO;

    public static class HashExtensions
    {
        public static bool TryRead(this Stream stream, out Hash hash)
        {
            hash = null;
            if (!stream.TryRead(out byte[] hashCode))
                return false;
            if (hashCode.Length > Hash.NumBytes)
                return false;
            hash = new Hash(hashCode);
            return true;
        }

        public static void Write(this Stream stream, Hash hash) => stream.Write(hash.HashCode);
    }
}

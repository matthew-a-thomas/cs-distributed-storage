namespace Server.Models
{
    using System.IO;
    using DistributedStorage.Common;

    public static class OwnerExtensions
    {
        public static bool TryRead(this Stream stream, out Owner owner)
        {
            owner = null;
            if (!stream.TryRead(out string name))
                return false;
            owner = new Owner(name);
            return true;
        }

        public static void Write(this Stream stream, Owner owner)
        {
            stream.Write(owner.Identity);
        }
    }
}

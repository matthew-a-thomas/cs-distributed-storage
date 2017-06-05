namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using Encoding;

    public sealed class Node
    {
        public Manifest Manifest { get; set; }

        public RSAParameters Key { get; set; }

        public IList<Slice> Slices { get; set; }
    }
}

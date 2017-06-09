namespace ConsoleApp
{
    using System.Security.Cryptography;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage;

    internal sealed class Storage
    {
        public IFactoryContainer<Manifest, IContainer<Hash, Slice>> ContainersForManifests { get; set; }

        public IFile OurRsaKey { get; set; }

        public IContainer<Hash, RSAParameters> TrustedPublicKeys { get; set; }
    }
}

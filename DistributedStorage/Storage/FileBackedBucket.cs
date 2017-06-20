namespace DistributedStorage.Storage
{
    using System.Security.Cryptography;
    using Containers;
    using FileSystem;
    using Model;
    using Networking.Security;

    public sealed class FileBackedBucket : IBucket
    {
        public sealed class Options
        {
            public string OwnerIdentityFileName = "owner.rsa";
            public string PoolIdentityFileName = "pool.rsa";
            public string SelfIdentityFileName = "identity.rsa";
            public string SizeFileName = "size.long";
        }

        private readonly IDirectory _workingDirectory;
        private readonly Options _options;

        public FileBackedBucket(IDirectory workingDirectory, Options options = null)
        {
            _workingDirectory = workingDirectory;
            _options = options ?? new Options();
        }

        public IIdentity SelfIdentity => TryGetIdentity(_options.SelfIdentityFileName, out var identity) ? identity : null;
        public IIdentity OwnerIdentity => TryGetIdentity(_options.OwnerIdentityFileName, out var identity) ? identity : null;
        public IIdentity PoolIdentity => TryGetIdentity(_options.PoolIdentityFileName, out var identity) ? identity : null;
        public long Size { get; }

        private bool TryGetIdentity(string identityFileName, out RsaIdentity identity)
        {
            identity = null;
            if (!_workingDirectory.Files.TryGet(identityFileName, out var identityFile))
                return false;
            if (!identityFile.TryOpenRead(out var stream))
                return false;
            using (stream)
            {
                if (!stream.TryRead(out RSAParameters key))
                    return false;
                identity = new RsaIdentity(key);
                return true;
            }
        }

        public bool TryGetOwnerIdentity(out RsaIdentity ownerIdentity) => TryGetIdentity(_options.OwnerIdentityFileName, out ownerIdentity);

        public bool TryGetPoolIdentity(out RsaIdentity poolIdentity) => TryGetIdentity(_options.PoolIdentityFileName, out poolIdentity);

        public bool TryGetSelfIdentity(out RsaIdentity selfIdentity) => TryGetIdentity(_options.SelfIdentityFileName, out selfIdentity);

        private bool TryGetSize(out long size)
        {
            size = -1;
            if (!_workingDirectory.Files.TryGet(_options.SizeFileName, out var sizeFile))
                return false;
            if (!sizeFile.TryOpenRead(out var stream))
                return false;
            using (stream)
                return stream.TryRead(out size);
        }

        private bool TrySetIdentity(string identityFileName, RsaIdentity identity)
        {
            var identityFile = _workingDirectory.Files.GetOrCreate(identityFileName);
            if (!identityFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(identity.PrivateKey, true);
            return true;
        }

        public bool TrySetOwnerIdentity(RsaIdentity ownerIdentity) => TrySetIdentity(_options.OwnerIdentityFileName, ownerIdentity);

        public bool TrySetPoolIdentity(RsaIdentity poolIdentity) => TrySetIdentity(_options.PoolIdentityFileName, poolIdentity);

        public bool TrySetSelfIdentity(RsaIdentity selfIdentity) => TrySetIdentity(_options.SelfIdentityFileName, selfIdentity);
    }
}

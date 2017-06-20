namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Containers;
    using FileSystem;
    using Model;
    using Networking.Security;
    using DistributedStorage.Common;
    using Encoding;

    public sealed class FileBackedBucket : IBucket
    {
        public sealed class Options
        {
            public string ManifestExtension { get; set; } = ".manifest";
            public string ManifestsFolderName { get; set; } = "manifests";
            public string OwnerIdentityFileName { get; set; } = "owner.rsa";
            public string PoolIdentityFileName { get; set; } = "pool.rsa";
            public string SelfIdentityFileName { get; set; } = "identity.rsa";
            public string SizeFileName { get; set; } = "size.long";
            public string SliceExtension { get; set; } = ".slice";
        }

        #region Public properties

        public IIdentity SelfIdentity => TryGetSelfIdentity(out var identity) ? identity : null;
        public IIdentity OwnerIdentity => TryGetOwnerIdentity(out var identity) ? identity : null;
        public IIdentity PoolIdentity => TryGetPoolIdentity(out var identity) ? identity : null;
        public long Size => TryGetSize(out var size) ? size : 0;

        #endregion

        #region Private fields

        private readonly IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> _manifestFactoryContainer;
        private readonly Options _options;
        private readonly IDirectory _workingDirectory;

        #endregion

        public FileBackedBucket(IDirectory workingDirectory, Options options = null)
        {
            _options = options ?? new Options();
            _workingDirectory = workingDirectory;
            var manifestsFolder = _workingDirectory.Directories.GetOrCreate(_options.ManifestsFolderName);
            _manifestFactoryContainer = new ManifestsAndSlicesFactoryContainer(new ManifestsAndSlicesFactoryContainer.Options(_options.ManifestExtension, _options.SliceExtension, manifestsFolder));
        }

        #region Public methods

        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _manifestFactoryContainer.TryGet(forManifest, out var container) ? container.GetKeys() : Enumerable.Empty<Hash>();

        public IEnumerable<Manifest> GetManifests() => _manifestFactoryContainer.GetKeys();

        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes)
        {
            var hashLookup = new HashSet<Hash>(hashes);
            if (!_manifestFactoryContainer.TryGet(forManifest, out var container))
                return Enumerable.Empty<Slice>();
        }

        public bool TryGetOwnerIdentity(out RsaIdentity ownerIdentity) => TryGetIdentity(_options.OwnerIdentityFileName, out ownerIdentity);

        public bool TryGetPoolIdentity(out RsaIdentity poolIdentity) => TryGetIdentity(_options.PoolIdentityFileName, out poolIdentity);

        public bool TryGetSelfIdentity(out RsaIdentity selfIdentity) => TryGetIdentity(_options.SelfIdentityFileName, out selfIdentity);

        public bool TryGetSize(out long size)
        {
            size = -1;
            if (!_workingDirectory.Files.TryGet(_options.SizeFileName, out var sizeFile))
                return false;
            if (!sizeFile.TryOpenRead(out var stream))
                return false;
            using (stream)
                return stream.TryRead(out size);
        }

        public bool TrySetOwnerIdentity(RsaIdentity ownerIdentity) => TrySetIdentity(_options.OwnerIdentityFileName, ownerIdentity);

        public bool TrySetPoolIdentity(RsaIdentity poolIdentity) => TrySetIdentity(_options.PoolIdentityFileName, poolIdentity);

        public bool TrySetSelfIdentity(RsaIdentity selfIdentity) => TrySetIdentity(_options.SelfIdentityFileName, selfIdentity);

        public bool TrySetSize(long size)
        {
            var sizeFile = _workingDirectory.Files.GetOrCreate(_options.SizeFileName);
            if (!sizeFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(size);
            return true;
        }

        #endregion

        #region Private methods

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

        private bool TrySetIdentity(string identityFileName, RsaIdentity identity)
        {
            var identityFile = _workingDirectory.Files.GetOrCreate(identityFileName);
            if (!identityFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(identity.PrivateKey, true);
            return true;
        }

        #endregion
    }
}

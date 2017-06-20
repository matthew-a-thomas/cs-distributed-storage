namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using Containers;
    using FileSystem;
    using Model;
    using Networking.Security;
    using Common;
    using Encoding;

    /// <summary>
    /// An implementation of both <see cref="IBucket"/> and <see cref="IPoolBucket"/> that builds on an <see cref="IDirectory"/>
    /// </summary>
    public sealed class FileBackedBucket : IBucket, IPoolBucket
    {
        /// <summary>
        /// Different options for creating a <see cref="FileBackedBucket"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The extension that serialized <see cref="Manifest"/>s should have
            /// </summary>
            public string ManifestExtension { get; set; } = ".manifest";

            /// <summary>
            /// The name of the directory in which <see cref="Manifest"/>s and their associated <see cref="Slice"/>s are stored
            /// </summary>
            public string ManifestsFolderName { get; set; } = "manifests";

            /// <summary>
            /// The file name for the identity of this <see cref="FileBackedBucket"/>'s owner
            /// </summary>
            public string OwnerIdentityFileName { get; set; } = "owner.rsa";

            /// <summary>
            /// The file name for the identity of this <see cref="FileBackedBucket"/>'s pool
            /// </summary>
            public string PoolIdentityFileName { get; set; } = "pool.rsa";

            /// <summary>
            /// The file name for the identity of this <see cref="FileBackedBucket"/>
            /// </summary>
            public string SelfIdentityFileName { get; set; } = "identity.rsa";

            /// <summary>
            /// The file name storing the desired size of this <see cref="FileBackedBucket"/>
            /// </summary>
            public string SizeFileName { get; set; } = "size.long";

            /// <summary>
            /// The extension to use for individual <see cref="Slice"/>s
            /// </summary>
            public string SliceExtension { get; set; } = ".slice";
        }

        #region Public properties

        /// <summary>
        /// This <see cref="FileBackedBucket"/>'s identity
        /// </summary>
        public IIdentity SelfIdentity => TryGetSelfIdentity(out var identity) ? identity : null;

        /// <summary>
        /// The identity of the owner of this <see cref="FileBackedBucket"/>
        /// </summary>
        public IIdentity OwnerIdentity => TryGetOwnerIdentity(out var identity) ? identity : null;

        /// <summary>
        /// The identity of the pool to which this <see cref="FileBackedBucket"/> belongs
        /// </summary>
        public IIdentity PoolIdentity => TryGetPoolIdentity(out var identity) ? identity : null;

        /// <summary>
        /// The maximum size that this <see cref="FileBackedBucket"/> should be
        /// </summary>
        public long Size => TryGetSize(out var size) ? size : 0;

        #endregion

        #region Private fields

        private readonly IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> _manifestFactoryContainer;
        private readonly Options _options;
        private readonly IFactoryContainer<string, IFile> _files;

        #endregion

        #region Constructor

        public FileBackedBucket(IDirectory workingDirectory, Options options = null)
        {
            _options = options ?? new Options();
            _files = workingDirectory.Files;
            var manifestsFolder = workingDirectory.Directories.GetOrCreate(_options.ManifestsFolderName);
            _manifestFactoryContainer = new ManifestsAndSlicesFactoryContainer(new ManifestsAndSlicesFactoryContainer.Options(_options.ManifestExtension, _options.SliceExtension, manifestsFolder));
        }

        #endregion
        
        #region Public methods

        /// <summary>
        /// Adds the given <see cref="Slice"/>s into this <see cref="FileBackedBucket"/>, associating them with the given <see cref="Manifest"/>
        /// </summary>
        public void AddSlices(Manifest forManifest, Slice[] slices)
        {
            var container = _manifestFactoryContainer.GetOrCreate(forManifest);
            foreach (var slice in slices)
                container.TryAdd(slice.ComputeHash(), slice);
        }

        /// <summary>
        /// Deletes any of the <paramref name="hashesToDelete"/> which are associated with the given <see cref="Manifest"/>
        /// </summary>
        public void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete)
        {
            if (!_manifestFactoryContainer.TryGet(forManifest, out var container))
                return;
            foreach (var hash in hashesToDelete)
                container.TryRemove(hash);
        }

        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _manifestFactoryContainer.TryGet(forManifest, out var container) ? container.GetKeys() : Enumerable.Empty<Hash>();

        public IEnumerable<Manifest> GetManifests() => _manifestFactoryContainer.GetKeys();

        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes)
        {
            if (!_manifestFactoryContainer.TryGet(forManifest, out var container))
                yield break;
            foreach (var hash in hashes)
            {
                if (!container.TryGet(hash, out var slice))
                    continue;
                yield return slice;
            }
        }

        public bool TryGetOwnerIdentity(out RsaIdentity ownerIdentity) => TryGetIdentity(_options.OwnerIdentityFileName, out ownerIdentity);

        public bool TryGetPoolIdentity(out RsaIdentity poolIdentity) => TryGetIdentity(_options.PoolIdentityFileName, out poolIdentity);

        public bool TryGetSelfIdentity(out RsaIdentity selfIdentity) => TryGetIdentity(_options.SelfIdentityFileName, out selfIdentity);

        public bool TryGetSize(out long size)
        {
            size = -1;
            if (!_files.TryGet(_options.SizeFileName, out var sizeFile))
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
            var sizeFile = _files.GetOrCreate(_options.SizeFileName);
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
            if (!_files.TryGet(identityFileName, out var identityFile))
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
            var identityFile = _files.GetOrCreate(identityFileName);
            if (!identityFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(identity.PrivateKey, true);
            return true;
        }

        #endregion
    }
}

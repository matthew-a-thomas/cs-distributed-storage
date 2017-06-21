﻿namespace DistributedStorage.Storage
{
    using System;
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
    public sealed class FileBackedBucket : IBucket<RsaIdentity>, IPoolBucket
    {
        public sealed class Factory
        {
            /// <summary>
            /// Tries to create a new <see cref="FileBackedBucket"/> within the given <paramref name="workingDirectory"/>
            /// </summary>
            public bool TryCreateNew(IDirectory workingDirectory, RsaIdentity ownerIdentity, RsaIdentity poolIdentity, RsaIdentity selfIdentity, long maxSize, out FileBackedBucket bucket, Options options = null)
            {
                bucket = new FileBackedBucket(workingDirectory, options);
                return
                    TryInitializeIdentity(bucket, bucket._options.SelfIdentityFileName, selfIdentity)
                    &&
                    TryInitializeIdentity(bucket, bucket._options.OwnerIdentityFileName, ownerIdentity)
                    &&
                    TryInitializeIdentity(bucket, bucket._options.PoolIdentityFileName, poolIdentity)
                    &&
                    TryInitializeMaxSize(bucket, maxSize);
            }
            
            /// <summary>
            /// Attempts to initialize the given <paramref name="identity"/> of the given <paramref name="bucket"/>
            /// </summary>
            private static bool TryInitializeIdentity(FileBackedBucket bucket, string identityFileName, RsaIdentity identity)
            {
                if (!bucket._files.TryCreate(identityFileName, out var identityFile))
                    return false;
                if (!identityFile.TryOpenWrite(out var stream))
                    return false;
                using (stream)
                    stream.Write(identity.PrivateKey, true);
                return true;
            }
            
            /// <summary>
            /// Attempts to initialize the max size of the given <paramref name="bucket"/>
            /// </summary>
            private static bool TryInitializeMaxSize(FileBackedBucket bucket, long maxSize)
            {
                if (!bucket._files.TryCreate(bucket._options.SizeFileName, out var sizeFile))
                    return false;
                if (!sizeFile.TryOpenWrite(out var stream))
                    return false;
                using (stream)
                    stream.Write(maxSize);
                return true;
            }
        }

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
        /// Lazily loads this <see cref="FileBackedBucket"/>'s identity
        /// </summary>
        public RsaIdentity SelfIdentity => _selfIdentityLazy.Value;

        /// <summary>
        /// Lazily loads the identity of the owner of this <see cref="FileBackedBucket"/>
        /// </summary>
        public RsaIdentity OwnerIdentity => _ownerIdentityLazy.Value;

        /// <summary>
        /// Lazily loads the identity of the pool to which this <see cref="FileBackedBucket"/> belongs
        /// </summary>
        public RsaIdentity PoolIdentity => _poolIdentityLazy.Value;

        /// <summary>
        /// Lazily loads the maximum size that this <see cref="FileBackedBucket"/> should be
        /// </summary>
        public long MaxSize => _maxSizeLazy.Value;

        #endregion

        #region Private fields

        private readonly IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> _manifestFactoryContainer;
        private readonly Options _options;
        private readonly IFactoryContainer<string, IFile> _files;
        private readonly IDirectory _workingDirectory;
        private readonly Lazy<RsaIdentity>
            _ownerIdentityLazy,
            _poolIdentityLazy,
            _selfIdentityLazy;
        private readonly Lazy<long>
            _maxSizeLazy;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="FileBackedBucket"/> that is based on the given <paramref name="workingDirectory"/>
        /// </summary>
        public FileBackedBucket(IDirectory workingDirectory, Options options = null)
        {
            _options = options ?? new Options();
            _workingDirectory = workingDirectory;
            _files = workingDirectory.Files;
            var manifestsFolder = workingDirectory.Directories.GetOrCreate(_options.ManifestsFolderName);
            _manifestFactoryContainer = new ManifestsAndSlicesFactoryContainer(new ManifestsAndSlicesFactoryContainer.Options(_options.ManifestExtension, _options.SliceExtension, manifestsFolder));

            _ownerIdentityLazy = new Lazy<RsaIdentity>(() => TryGetIdentity(_options.OwnerIdentityFileName, out var identity) ? identity : null);
            _poolIdentityLazy = new Lazy<RsaIdentity>(() => TryGetIdentity(_options.PoolIdentityFileName, out var identity) ? identity : null);
            _selfIdentityLazy = new Lazy<RsaIdentity>(() => TryGetIdentity(_options.SelfIdentityFileName, out var identity) ? identity : null);
            _maxSizeLazy = new Lazy<long>(() => TryGetMaxSize(out var maxSize) ? maxSize : 0);
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

        public long GetCurrentSize() => _workingDirectory.GetCurrentSize();

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

        #endregion

        #region Private methods
        
        private bool TryGetMaxSize(out long maxSize)
        {
            maxSize = -1;
            if (!_files.TryGet(_options.SizeFileName, out var sizeFile))
                return false;
            if (!sizeFile.TryOpenRead(out var stream))
                return false;
            using (stream)
                return stream.TryRead(out maxSize);
        }

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

        #endregion
    }
}


namespace DistributedStorage.Model
{
    using System.Collections.Generic;
    using Common;
    using Encoding;

    public sealed class Bucket<TIdentity> : IBucket<TIdentity>
        where TIdentity : IIdentity
    {
        public delegate long GetCurrentSizeDelegate();
        public delegate IEnumerable<Hash> GetHashesDelegate(Manifest forManifest);
        public delegate IEnumerable<Manifest> GetManifestsDelegate();
        public delegate IEnumerable<Slice> GetSlicesDelegate(Manifest forManifest, Hash[] hashes);

        public sealed class Options
        {
            public GetCurrentSizeDelegate GetCurrentSize { get; }
            public GetHashesDelegate GetHashes { get; }
            public GetManifestsDelegate GetManifests { get; }
            public GetSlicesDelegate GetSlices { get; }
            public TIdentity OwnerIdentity { get; }
            public TIdentity PoolIdentity { get; }
            public TIdentity SelfIdentity { get; }
            public long Size { get; }

            public Options(GetHashesDelegate getHashes, GetManifestsDelegate getManifests, GetSlicesDelegate getSlices, TIdentity ownerIdentity, TIdentity poolIdentity, TIdentity selfIdentity, long size, GetCurrentSizeDelegate getCurrentSize)
            {
                GetHashes = getHashes;
                GetManifests = getManifests;
                GetSlices = getSlices;
                OwnerIdentity = ownerIdentity;
                PoolIdentity = poolIdentity;
                SelfIdentity = selfIdentity;
                Size = size;
                GetCurrentSize = getCurrentSize;
            }
        }

        public TIdentity SelfIdentity => _options.SelfIdentity;
        public TIdentity OwnerIdentity => _options.OwnerIdentity;
        public TIdentity PoolIdentity => _options.PoolIdentity;
        public long MaxSize => _options.Size;

        private readonly Options _options;

        public Bucket(Options options)
        {
            _options = options;
        }

        public long GetCurrentSize() => _options.GetCurrentSize();

        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _options.GetHashes(forManifest);

        public IEnumerable<Manifest> GetManifests() => _options.GetManifests();

        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes) => _options.GetSlices(forManifest, hashes);
    }
}

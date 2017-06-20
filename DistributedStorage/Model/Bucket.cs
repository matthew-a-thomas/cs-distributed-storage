
namespace DistributedStorage.Model
{
    using System.Collections.Generic;
    using Common;
    using Encoding;

    public sealed class Bucket : IBucket
    {
        public delegate IEnumerable<Hash> GetHashesDelegate(Manifest forManifest);
        public delegate IEnumerable<Manifest> GetManifestsDelegate();
        public delegate IEnumerable<Slice> GetSlicesDelegate(Manifest forManifest, Hash[] hashes);

        public sealed class Options
        {
            public GetHashesDelegate GetHashes { get; }
            public GetManifestsDelegate GetManifests { get; }
            public GetSlicesDelegate GetSlices { get; }
            public IIdentity OwnerIdentity { get; }
            public IIdentity PoolIdentity { get; }
            public IIdentity SelfIdentity { get; }
            public long Size { get; }

            public Options(GetHashesDelegate getHashes, GetManifestsDelegate getManifests, GetSlicesDelegate getSlices, IIdentity ownerIdentity, IIdentity poolIdentity, IIdentity selfIdentity, long size)
            {
                GetHashes = getHashes;
                GetManifests = getManifests;
                GetSlices = getSlices;
                OwnerIdentity = ownerIdentity;
                PoolIdentity = poolIdentity;
                SelfIdentity = selfIdentity;
                Size = size;
            }
        }

        public IIdentity SelfIdentity => _options.SelfIdentity;
        public IIdentity OwnerIdentity => _options.OwnerIdentity;
        public IIdentity PoolIdentity => _options.PoolIdentity;
        public long Size => _options.Size;

        private readonly Options _options;

        public Bucket(Options options)
        {
            _options = options;
        }

        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _options.GetHashes(forManifest);

        public IEnumerable<Manifest> GetManifests() => _options.GetManifests();

        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes) => _options.GetSlices(forManifest, hashes);
    }
}

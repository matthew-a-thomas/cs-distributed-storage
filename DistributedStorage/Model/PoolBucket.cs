namespace DistributedStorage.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Encoding;

    public sealed class PoolBucket : IPoolBucket
    {
        private readonly Options _options;

        public delegate void AddSlicesDelegate(Manifest forManifest, Slice[] slices);
        public delegate void DeleteSlicesDelegate(Manifest forManifest, Hash[] hashesToDelete);
        public delegate IEnumerable<Hash> GetHashesDelegate(Manifest forManifest);
        public delegate IEnumerable<Manifest> GetManifestsDelegate();
        public delegate IEnumerable<Slice> GetSlicesDelegate(Manifest forManifest, Hash[] hashes);

        public sealed class Options
        {
            public AddSlicesDelegate AddSlices { get; set; } = (x, y) => { };
            public DeleteSlicesDelegate DeleteSlices { get; set; } = (x, y) => { };
            public GetHashesDelegate GetHashes { get; set; } = x => Enumerable.Empty<Hash>();
            public GetManifestsDelegate GetManifests { get; set; } = () => Enumerable.Empty<Manifest>();
            public GetSlicesDelegate GetSlices { get; set; } = (x, y) => Enumerable.Empty<Slice>();
        }

        public PoolBucket(Options options)
        {
            _options = options;
        }

        public void AddSlices(Manifest forManifest, Slice[] slices) => _options.AddSlices(forManifest, slices);

        public void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete) => _options.DeleteSlices(forManifest, hashesToDelete);

        public IEnumerable<Hash> GetHashes(Manifest forManifest) => _options.GetHashes(forManifest);

        public IEnumerable<Manifest> GetManifests() => _options.GetManifests();

        public IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes) => _options.GetSlices(forManifest, hashes);
    }
}

namespace DistributedStorage.Model
{
    using Common;
    using Encoding;

    public sealed class PoolBucket : IPoolBucket
    {
        private readonly Options _options;

        public delegate void AddSlicesDelegate(Manifest forManifest, Slice[] slices);
        public delegate void DeleteSlicesDelegate(Manifest forManifest, Hash[] hashesToDelete);

        public sealed class Options
        {
            public AddSlicesDelegate AddSlices { get; set; } = (x, y) => { };
            public DeleteSlicesDelegate DeleteSlices { get; set; } = (x, y) => { };
        }

        public PoolBucket(Options options)
        {
            _options = options;
        }

        public void AddSlices(Manifest forManifest, Slice[] slices) => _options.AddSlices(forManifest, slices);

        public void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete) => _options.DeleteSlices(forManifest, hashesToDelete);
    }
}

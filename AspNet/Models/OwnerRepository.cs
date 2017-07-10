namespace AspNet.Models
{
    using DistributedStorage.Storage.Containers;
    using DistributedStorage.Storage.FileSystem;

    public sealed class OwnerRepository
    {
        public sealed class Options
        {
            public Options(string ownerFileName)
            {
                OwnerFileName = ownerFileName;
            }

            public string OwnerFileName { get; }
        }
        
        private readonly IDirectory _appDataDirectory;
        private readonly Options _options;

        private IFile OwnerFile => _appDataDirectory.Files.GetOrCreate(_options.OwnerFileName);

        public OwnerRepository(IDirectory appDataDirectory, Options options)
        {
            _appDataDirectory = appDataDirectory;
            _options = options;
        }

        public bool TryGetOwner(out Owner owner)
        {
            owner = null;
            if (!OwnerFile.TryOpenRead(out var stream))
                return false;
            using (stream)
                return stream.TryRead(out owner);
        }

        public bool TrySetOwner(Owner owner)
        {
            if (!OwnerFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(owner);
            return true;
        }
    }
}

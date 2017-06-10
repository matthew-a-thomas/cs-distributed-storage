namespace DistributedStorage.Storage
{
    using Containers;

    public sealed class Directory : IDirectory
    {
        public sealed class Options
        {
            public IFactoryContainer<string, IDirectory> Directories { get; set; }

            public IFactoryContainer<string, IFile> Files { get; set; }
        }

        private readonly Options _options;

        public Directory(Options options)
        {
            _options = options;
        }

        public IFactoryContainer<string, IDirectory> Directories => _options.Directories;
        public IFactoryContainer<string, IFile> Files => _options.Files;
    }
}

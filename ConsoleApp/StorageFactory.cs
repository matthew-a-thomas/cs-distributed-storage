namespace ConsoleApp
{
    using DistributedStorage.Storage;

    internal sealed class StorageFactory
    {
        public sealed class Dependencies
        {
            public IFactoryContainer<string, IFactoryContainer<string, IFile>> WorkingDirectory { get; }

            public Dependencies(IFactoryContainer<string, IFactoryContainer<string, IFile>> workingDirectory)
            {
                WorkingDirectory = workingDirectory;
            }
        }

        public sealed class Options
        {
            public string OurRsaKeyName { get; set; } = "ours.rsa";
            public string OurRsaFolderName { get; set; } = "private";
            public string ManifestsFolderName { get; set; } = "manifests";
            public string TrustedPublicKeysFolderName { get; set; } = "trusted";
        }

        private readonly Dependencies _dependencies;

        public StorageFactory(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public Storage CreateStorage(Options options = null)
        {
            options = options ?? new Options();
            return new Storage
            {
                
            }
        }
    }
}

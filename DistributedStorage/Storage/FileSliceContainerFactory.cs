namespace DistributedStorage.Storage
{
    using Common;
    using Encoding;
    using IFolder = IFactoryContainer<string, IFile>;

    public sealed class FileSliceContainerFactory
    {
        private readonly IFactoryContainer<string, IFolder> _baseContainer;

        public FileSliceContainerFactory(IFactoryContainer<string, IFolder> baseContainer)
        {
            _baseContainer = baseContainer;
        }

        public FileSliceContainer CreateSliceContainer(Manifest forManifest)
        {
            // Create a subdirectory for slices connected to this manifest
            var workingFolder = _baseContainer.GetOrCreate(forManifest.Id.HashCode.ToHex());

            // Return a new FileSliceContainer
            return new FileSliceContainer(workingFolder);
        }

    }
}

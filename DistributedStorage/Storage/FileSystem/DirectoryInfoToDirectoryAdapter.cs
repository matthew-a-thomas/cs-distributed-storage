namespace DistributedStorage.Storage.FileSystem
{
    using System.IO;
    using Containers;

    public sealed class DirectoryInfoToDirectoryAdapter : IDirectory
    {
        public DirectoryInfoToDirectoryAdapter(DirectoryInfo directory)
        {
            Directories = new DirectoryInfoToDirectoryFactoryContainerAdapter(directory);
            Files = new DirectoryInfoToFilesFactoryContainerAdapter(directory);
        }

        public IFactoryContainer<string, IDirectory> Directories { get; }
        public IFactoryContainer<string, IFile> Files { get; }
    }
}

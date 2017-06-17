namespace DistributedStorage.Storage.FileSystem
{
    using Containers;

    public interface IDirectory
    {
        IFactoryContainer<string, IDirectory> Directories { get; }
        IFactoryContainer<string, IFile> Files { get; }
    }
}

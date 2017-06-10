namespace DistributedStorage.Storage
{
    public interface IDirectory
    {
        IFactoryContainer<string, IDirectory> Directories { get; }
        IFactoryContainer<string, IFile> Files { get; }
    }
}

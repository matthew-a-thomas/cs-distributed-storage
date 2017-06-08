namespace DistributedStorage.Storage
{
    using System.IO;

    public interface IFile
    {
        bool TryOpenRead(out Stream stream);
        bool TryOpenWrite(out Stream stream);
    }
}

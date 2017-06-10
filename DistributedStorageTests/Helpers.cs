namespace DistributedStorageTests
{
    using DistributedStorage.Common;
    using DistributedStorage.Storage.Containers;
    using DistributedStorage.Storage.FileSystem;

    internal static class Helpers
    {
        public static IFile CreateFile() => new byte[10 * 1024].ToFile();

        public static IDirectory CreateDirectory()
        {
            var directories = new MemoryFactoryContainer<string, IDirectory>(CreateDirectory);
            var files = new MemoryFactoryContainer<string, IFile>(CreateFile);
            var dir = new Directory(new Directory.Options
            {
                Directories = directories,
                Files = files
            });
            return dir;
        }
    }
}

namespace DistributedStorageTests
{
    using DistributedStorage.Common;
    using DistributedStorage.Networking;
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

        /// <summary>
        /// Creates a new dummy <see cref="ISerializer{T}"/>, which always returns true, default(T), and new byte[0]
        /// </summary>
        public static ISerializer<T> CreateDummySerializer<T>()
        {
            byte[] Serialize(T thing) => new byte[0];
            bool TryDeserialize(byte[] bytes, out T thing)
            {
                thing = default(T);
                return true;
            }
            var serializer = new Serializer<T>(new Serializer<T>.Options(Serialize, TryDeserialize));
            return serializer;
        }
    }
}

namespace DistributedStorage.Storage.FileSystem
{
    using System.IO;

    public static class DirectoryExtensions
    {
        /// <summary>
        /// Converts this <see cref="DirectoryInfo"/> into an <see cref="IDirectory"/>
        /// </summary>
        public static IDirectory ToDirectory(this DirectoryInfo directoryInfo) => new DirectoryInfoToDirectoryAdapter(directoryInfo);
    }
}

namespace DistributedStorage.Storage.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Containers;

    public static class DirectoryExtensions
    {
        /// <summary>
        /// Recursively calculates the total size of all files contained within this <see cref="IDirectory"/>
        /// </summary>
        public static long GetCurrentSize(this IDirectory directory)
        {
            var directories = new Stack<IDirectory>();
            var total = 0L;
            directories.Push(directory);
            while (directories.Count > 0)
            {
                var d = directories.Pop();
                total += d.Files.GetValues().Sum(file => file.GetCurrentSize());
                foreach (var subdirectory in d.Directories.GetValues())
                    directories.Push(subdirectory);
            }
            return total;
        }

        /// <summary>
        /// Converts this <see cref="DirectoryInfo"/> into an <see cref="IDirectory"/>
        /// </summary>
        public static IDirectory ToDirectory(this DirectoryInfo directoryInfo) => new DirectoryInfoToDirectoryAdapter(directoryInfo);
    }
}

namespace DistributedStorage.Storage.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Containers;

    /// <summary>
    /// Wraps a <see cref="DirectoryInfo"/> as an <see cref="IFactoryContainer{TKey, TValue}"/>
    /// </summary>
    public class DirectoryInfoToFilesFactoryContainerAdapter : IFactoryContainer<string, IFile>
    {
        /// <summary>
        /// The wrapped <see cref="DirectoryInfo"/>
        /// </summary>
        private readonly DirectoryInfo _directory;

        /// <summary>
        /// Creates a new <see cref="DirectoryInfoToFilesFactoryContainerAdapter"/>, which wraps the given <paramref name="directory"/> as an <see cref="IFactoryContainer{TKey, TValue}"/>
        /// </summary>
        public DirectoryInfoToFilesFactoryContainerAdapter(DirectoryInfo directory)
        {
            _directory = directory;
        }

        /// <summary>
        /// Produces a <see cref="FileInfo"/> for the file within the <see cref="_directory"/> having the given <paramref name="filename"/>
        /// </summary>
        private FileInfo GetFileInfoFor(string filename) => new FileInfo(Path.Combine(_directory.FullName, filename));

        /// <summary>
        /// Enumerates all file names that currently exist in the wrapped <see cref="DirectoryInfo"/>
        /// </summary>
        public IEnumerable<string> GetKeys() => _directory.EnumerateFiles().Select(file => file.Name);

        /// <summary>
        /// Creates a new <see cref="IFile"/> for the file having the given <paramref name="key"/>, if it exists
        /// </summary>
        public bool TryGet(string key, out IFile value)
        {
            var file = GetFileInfoFor(key);
            value = null;
            if (!file.Exists)
                return false;
            value = new FileInfoToFileAdapter(file);
            return true;
        }

        /// <summary>
        /// Tries to delete the named with the given <paramref name="key"/>, if it exists
        /// </summary>
        public bool TryRemove(string key)
        {
            var file = GetFileInfoFor(key);
            if (!file.Exists)
                return false;
            file.Delete();
            return true;
        }

        /// <summary>
        /// Creates a new file in the wrapped <see cref="DirectoryInfo"/> and returns a new <see cref="IFile"/>, if that file doesn't already exist
        /// </summary>
        public bool TryCreate(string key, out IFile value)
        {
            var file = GetFileInfoFor(key);
            value = null;
            if (file.Exists)
                return false;
            using (file.Create()) { }
            value = new FileInfoToFileAdapter(file);
            return true;
        }
    }
}
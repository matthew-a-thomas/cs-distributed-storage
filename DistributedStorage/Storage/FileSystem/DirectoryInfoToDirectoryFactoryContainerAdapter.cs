namespace DistributedStorage.Storage.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Containers;

    /// <summary>
    /// Something that can adapt a <see cref="DirectoryInfo"/> into an <see cref="IFactoryContainer{TKey, TValue}"/>
    /// </summary>
    public sealed class DirectoryInfoToDirectoryFactoryContainerAdapter : IFactoryContainer<string, IDirectory>
    {
        private readonly DirectoryInfo _directory;

        /// <summary>
        /// Creates a new <see cref="DirectoryInfoToDirectoryFactoryContainerAdapter"/> that wraps the given <see cref="DirectoryInfo"/>
        /// </summary>
        public DirectoryInfoToDirectoryFactoryContainerAdapter(DirectoryInfo directory)
        {
            _directory = directory;
        }

        /// <summary>
        /// Returns all directory names that are currently in the wrapped <see cref="DirectoryInfo"/>
        /// </summary>
        public IEnumerable<string> GetKeys() => _directory.EnumerateDirectories().Select(directory => directory.Name);

        /// <summary>
        /// Produces a <see cref="DirectoryInfo"/> for a subdirectory that would have the given <paramref name="name"/>
        /// </summary>
        private DirectoryInfo GetSubDirectory(string name) => new DirectoryInfo(Path.Combine(_directory.FullName, name));

        /// <summary>
        /// Tries to get an existing <see cref="IDirectory"/> for the given key.
        /// False will be returned if no subdirectory with the given name currently exists
        /// </summary>
        public bool TryGet(string key, out IDirectory value)
        {
            var directory = GetSubDirectory(key);
            value = null;
            if (!directory.Exists)
                return false;
            value = new DirectoryInfoToDirectoryAdapter(directory);
            return true;
        }

        /// <summary>
        /// Tries to remove the given subdirectory (and all its contents).
        /// Returns false if the subdirectory doesn't exist
        /// </summary>
        public bool TryRemove(string key)
        {
            var directory = GetSubDirectory(key);
            var existedBefore = directory.Exists;
            if (!existedBefore)
                return false;
            directory.Delete(true);
            return true;
        }

        /// <summary>
        /// Tries to create a new subdirectory named with the given <paramref name="key"/>.
        /// Returns false if that subdirectory already exists
        /// </summary>
        public bool TryCreate(string key, out IDirectory value)
        {
            var directory = GetSubDirectory(key);
            value = null;
            if (directory.Exists)
                return false;
            directory.Create();
            value = new DirectoryInfoToDirectoryAdapter(directory);
            return true;
        }
    }
}

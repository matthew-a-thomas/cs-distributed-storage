namespace DistributedStorage.Storage.FileSystem
{
    using System.IO;

    /// <summary>
    /// Wraps a <see cref="FileInfo"/> as an <see cref="IFile"/>
    /// </summary>
    public sealed class FileInfoToFileAdapter : IFile
    {
        /// <summary>
        /// The wrapped <see cref="FileInfo"/>
        /// </summary>
        private readonly FileInfo _file;

        /// <summary>
        /// Creates a new <see cref="FileInfoToFileAdapter"/>, which wraps the given <paramref name="file"/> as an <see cref="IFile"/>
        /// </summary>
        public FileInfoToFileAdapter(FileInfo file)
        {
            _file = file;
        }

        /// <summary>
        /// Returns the current size of this file
        /// </summary>
        public long GetCurrentSize()
        {
            _file.Refresh();
            return _file.Length;
        }

        /// <summary>
        /// Opens a read-only stream for the wrapped <see cref="FileInfo"/>, if the file exists
        /// </summary>
        public bool TryOpenRead(out Stream stream)
        {
            _file.Refresh();
            stream = null;
            if (!_file.Exists)
                return false;
            stream = _file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return true;
        }

        /// <summary>
        /// Opens a read-write stream for the wrapped <see cref="FileInfo"/>, if the file exists
        /// </summary>
        public bool TryOpenWrite(out Stream stream)
        {
            _file.Refresh();
            stream = null;
            if (!_file.Exists)
                return false;
            stream = _file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return true;
        }
    }
}

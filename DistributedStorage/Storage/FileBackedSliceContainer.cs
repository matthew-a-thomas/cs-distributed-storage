namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using Encoding;

    /// <summary>
    /// An implementation of <see cref="ISliceContainer"/> that reads and writes <see cref="Slice"/>s from/to a directory on disk
    /// </summary>
    public sealed class FileBackedSliceContainer : ISliceContainer
    {
        #region Constants

        /// <summary>
        /// The file extension given to all <see cref="Slice"/>s when they are written to disk
        /// </summary>
        private const string SliceExtension = ".slice";

        #endregion

        #region Private fields

        /// <summary>
        /// The directory into which and from which <see cref="Slice"/>s are written and read
        /// </summary>
        private readonly DirectoryInfo _workingDirectory;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ISliceContainer"/> that gets and stores <see cref="Slice"/>s from/to the given <paramref name="workingDirectory"/>
        /// </summary>
        public FileBackedSliceContainer(DirectoryInfo workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the given <paramref name="slice"/> out to this <see cref="FileBackedSliceContainer"/>'s working directory
        /// </summary>
        public void AddSlice(Slice slice)
        {
            var hash = slice.ComputeHash();
            var filename = Path.Combine(_workingDirectory.FullName, $"{hash.HashCode.ToHex()}{SliceExtension}");
            using (var stream = File.OpenWrite(filename))
            {
                stream.Write(slice);
            }
        }

        /// <summary>
        /// Iterates through all <see cref="Slice"/>s in this <see cref="FileBackedSliceContainer"/>'s working directory
        /// </summary>
        public IEnumerable<Slice> GetSlices()
        {
            foreach (var file in _workingDirectory.EnumerateFiles($"*{SliceExtension}"))
            {
                Slice slice;
                using (var stream = file.OpenRead())
                {
                    if (!stream.TryImmediateRead(out slice))
                        continue;
                }
                yield return slice;
            }
        }

        #endregion
    }
}

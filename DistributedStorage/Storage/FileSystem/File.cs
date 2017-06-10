namespace DistributedStorage.Storage.FileSystem
{
    using System.IO;

    /// <summary>
    /// An injected implementation of <see cref="IFile"/>
    /// </summary>
    public sealed class File : IFile
    {
        /// <summary>
        /// Delegate that can be used for <see cref="IFile.TryOpenRead"/>
        /// </summary>
        public delegate bool TryOpenReadDelegate(out Stream stream);

        /// <summary>
        /// Delegate that can be used for <see cref="IFile.TryOpenWrite"/>
        /// </summary>
        public delegate bool TryOpenWriteDelegate(out Stream stream);

        /// <summary>
        /// Different options for injecting a <see cref="File"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The method to use for <see cref="File.TryOpenRead"/>
            /// </summary>
            public TryOpenReadDelegate TryOpenRead { get; set; } = DefaultTry;

            /// <summary>
            /// The method to use for <see cref="File.TryOpenWrite"/>
            /// </summary>
            public TryOpenWriteDelegate TryOpenWrite { get; set; } = DefaultTry;

            /// <summary>
            /// A default method that does nothing and returns false
            /// </summary>
            private static bool DefaultTry(out Stream stream)
            {
                stream = null;
                return false;
            }
        }

        /// <summary>
        /// Different things we're injected with
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Creates a new <see cref="File"/>, using the given <paramref name="options"/> to inject behavior
        /// </summary>
        public File(Options options = null)
        {
            _options = options ?? new Options();
        }

        public bool TryOpenRead(out Stream stream) => _options.TryOpenRead(out stream);

        public bool TryOpenWrite(out Stream stream) => _options.TryOpenWrite(out stream);
    }
}

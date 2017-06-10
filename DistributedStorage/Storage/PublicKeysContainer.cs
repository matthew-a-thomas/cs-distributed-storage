namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Common;
    using Containers;
    using FileSystem;
    using Networking.Security;

    /// <summary>
    /// An <see cref="IAddableContainer{TKey,TValue}"/> that stores public RSA keys
    /// </summary>
    public sealed class PublicKeysContainer : IAddableContainer<Hash, RSAParameters>
    {
        /// <summary>
        /// The things that are needed to create a new <see cref="PublicKeysContainer"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The extension to use for key files
            /// </summary>
            public string RsaKeyExtension { get; }

            /// <summary>
            /// The directory which holds all the keys
            /// </summary>
            public IDirectory Directory { get; }

            /// <summary>
            /// Creates a new <see cref="Options"/> instance that can be used for creating new <see cref="PublicKeysContainer"/>s
            /// </summary>
            public Options(string rsaKeyExtension, IDirectory directory)
            {
                RsaKeyExtension = rsaKeyExtension;
                Directory = directory;
            }
        }

        /// <summary>
        /// The different options needed for a <see cref="PublicKeysContainer"/>
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Creates a new <see cref="PublicKeysContainer"/>, which stores public RSA keys
        /// </summary>
        public PublicKeysContainer(Options options)
        {
            _options = options;
        }
        
        /// <summary>
        /// Enumerates all the hashes of trusted RSA keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Hash> GetKeys()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var filename in _options.Directory.Files.GetKeys().Where(name => name.EndsWith(_options.RsaKeyExtension)).Select(Path.GetFileNameWithoutExtension))
            {
                if (!filename.TryToBytes(out var bytes))
                    continue;
                if (bytes.Length > Hash.NumBytes)
                    continue;
                yield return new Hash(bytes);
            }
        }

        /// <summary>
        /// A function for getting the filename of the given Hash
        /// </summary>
        private static string GetFilenameFor(Hash hash) => hash.HashCode.ToHex();

        /// <summary>
        /// Adds new trusted <see cref="RSAParameters"/>
        /// </summary>
        public bool TryAdd(Hash hash, RSAParameters key)
        {
            if (!_options.Directory.Files.TryCreate(GetFilenameFor(hash), out var file))
                return false;
            if (!file.TryOpenWrite(out var stream))
                return false;
            using (stream)
            {
                stream.Write(key);
                return true;
            }
        }

        /// <summary>
        /// Gets existing RSA parameters for the given <paramref name="hash"/>
        /// </summary>
        public bool TryGet(Hash hash, out RSAParameters key)
        {
            key = default(RSAParameters);
            if (!_options.Directory.Files.TryGet(GetFilenameFor(hash), out var file))
                return false;
            if (!file.TryOpenRead(out var stream))
                return false;
            using (stream)
            {
                return stream.TryImmediateRead(out key);
            }
        }

        /// <summary>
        /// Removes existing RSA parameters for the given hash
        /// </summary>
        public bool TryRemove(Hash hash) => _options.Directory.Files.TryRemove(GetFilenameFor(hash));
    }
}

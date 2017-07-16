namespace Server.Models
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.FileSystem;

    public sealed class SecretRepository
    {
        public sealed class Options
        {
            public Options(string secretFileName)
            {
                SecretFileName = secretFileName;
            }

            public string SecretFileName { get; }
        }

        private readonly ICache<byte[]> _cache;
        private readonly IDirectory _directory;
        private readonly Options _options;

        public SecretRepository(IDirectory directory, Options options)
        {
            _directory = directory;
            _options = options;
            _cache = new WeakCache<byte[]>(GetUncachedSecret);
        }

        /// <summary>
        /// Gets the cached secret, if possible
        /// </summary>
        public byte[] GetCachedSecret() => _cache.GetValue();

        /// <summary>
        /// Reads the secret from the <see cref="IFile"/>
        /// </summary>
        private byte[] GetUncachedSecret()
        {
            var file = GetSecretFile();
            if (!file.TryOpenRead(out var stream))
                return null;
            using (stream)
            {
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    var secret = memory.ToArray();
                    return secret;
                }
            }
        }

        private IFile GetSecretFile()
        {
            while (true)
            {
                if (_directory.Files.TryGet(_options.SecretFileName, out var secretFile))
                    return secretFile;
                if (!_directory.Files.TryCreate(_options.SecretFileName, out secretFile))
                    continue;
                if (!secretFile.TryOpenWrite(out var stream))
                    throw new Exception("Unable to open the secret file for writing");
                using (stream)
                {
                    var initialSecret = new byte[32];
                    using (var rng = RandomNumberGenerator.Create())
                        rng.GetBytes(initialSecret);
                    stream.Write(initialSecret, 0, initialSecret.Length);
                    return secretFile;
                }
            }
        }
    }
}

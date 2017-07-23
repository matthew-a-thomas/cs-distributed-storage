namespace Client.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Authentication;
    using DistributedStorage.Storage.Containers;
    using DistributedStorage.Storage.FileSystem;

    public sealed class UriToCredentialFileBasedContainer : IAddableContainer<Uri, Credential>
    {
        public sealed class Options
        {
            public string CredentialFileName { get; set; } = "credential";
        }

        private readonly Options _options;
        private readonly IDirectory _workingDirectory;

        public UriToCredentialFileBasedContainer(IDirectory workingDirectory, Options options = null)
        {
            _options = options ?? new Options();
            _workingDirectory = workingDirectory;
        }

        public IEnumerable<Uri> GetKeys() => _workingDirectory.Directories.GetKeys().Select(x => new Uri(x));

        public bool TryAdd(Uri key, Credential value)
        {
            if (!_workingDirectory.Directories.TryCreate(key.OriginalString, out var directory))
                return false;
            if (!directory.Files.TryCreate(_options.CredentialFileName, out var credentialFile))
                return false;
            if (!credentialFile.TryOpenWrite(out var stream))
                return false;
            using (stream)
                stream.Write(value);
            return true;
        }

        public bool TryGet(Uri key, out Credential value)
        {
            value = null;
            if (!_workingDirectory.Directories.TryGet(key.OriginalString, out var directory))
                return false;
            if (!directory.Files.TryGet(_options.CredentialFileName, out var credentialFile))
                return false;
            if (!credentialFile.TryOpenRead(out var stream))
                return false;
            using (stream)
                return stream.TryRead(out value);
        }

        public bool TryRemove(Uri key) => _workingDirectory.Directories.TryRemove(key.OriginalString);
    }
}

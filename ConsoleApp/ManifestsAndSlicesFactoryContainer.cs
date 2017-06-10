namespace ConsoleApp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.Containers;

    public sealed class ManifestsAndSlicesFactoryContainer : IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>>
    {
        public sealed class Options
        {
            public string ManifestExtension { get; }
            public string SliceExtension { get; }
            public IDirectory ManifestsFolder { get; }

            public Options(string manifestExtension, string sliceExtension, IDirectory manifestsFolder)
            {
                ManifestExtension = manifestExtension;
                SliceExtension = sliceExtension;
                ManifestsFolder = manifestsFolder;
            }
        }

        private readonly Options _options;

        public ManifestsAndSlicesFactoryContainer(Options options)
        {
            _options = options;
        }

        // Set up the function to get a list of all manifests from the manifestsFolder
        public IEnumerable<Manifest> GetKeys() => _options.ManifestsFolder.Directories.GetValues()
            .Select(directory =>
            {
                foreach (var filename in directory.Files.GetKeys().Where(name => name.EndsWith(_options.ManifestExtension)))
                {
                    if (!directory.Files.TryGet(filename, out var file))
                        continue;
                    if (!file.TryOpenRead(out var stream))
                        continue;
                    using (stream)
                    {
                        if (!stream.TryImmediateRead(out Manifest manifest))
                            continue;
                        return manifest;
                    }
                }
                return null;
            })
            .Where(manifest => !ReferenceEquals(manifest, null));

        // Set up a function that will create a Slice container for a given slice directory
        private bool TryCreateContainerFor(IDirectory slicesDirectory, out IAddableContainer<Hash, Slice> container)
        {
            string GetSliceName(Hash hash) => $"{hash.HashCode.ToHex()}{_options.SliceExtension}";

            // Set up a function that will enumerate all available Slice Hashes
            IEnumerable<Hash> GetSliceKeys() => slicesDirectory.Files.GetKeys().Where(name => name.EndsWith(_options.SliceExtension)).Select(Path.GetFileNameWithoutExtension).Select(hex => new Hash(hex.ToBytes()));

            // Set up a function for adding new Slices (and their associated Hashes)
            bool TryAddSlice(Hash hash, Slice slice)
            {
                if (!slicesDirectory.Files.TryCreate(GetSliceName(hash), out var file))
                    return false;
                if (!file.TryOpenWrite(out var stream))
                    return false;
                using (stream)
                {
                    stream.Write(slice);
                }
                return true;
            }

            // Set up a function for retrieving a Slice by its associated Hash
            bool TryGetSlice(Hash hash, out Slice slice)
            {
                slice = null;
                if (!slicesDirectory.Files.TryGet(GetSliceName(hash), out var file))
                    return false;
                if (!file.TryOpenRead(out var stream))
                    return false;
                using (stream)
                {
                    return stream.TryImmediateRead(out slice);
                }
            }

            // Set up a function for removing a Slice by its associated hash
            bool TryRemoveSlice(Hash hash) => slicesDirectory.Files.TryRemove(GetSliceName(hash));

            // Return a new Container that uses all the above
            container = new Container<Hash, Slice>(new Container<Hash, Slice>.Options
            {
                GetKeys = GetSliceKeys,
                TryAdd = TryAddSlice,
                TryGet = TryGetSlice,
                TryRemove = TryRemoveSlice
            });
            return true;
        }

        // Set up a function for getting a directory name from a Manifest
        private static string GetDirectoryNameFor(Manifest manifest) => manifest.Id.HashCode.ToHex();

        // Set up a function for getting the name of the file that the given Manifest should be in
        private string GetFileNameFor(Manifest manifest) => $"{manifest.Id.HashCode.ToHex()}{_options.ManifestExtension}";

        // Set up the function to create a new container for hash slices associated with a manifest
        public bool TryCreate(Manifest manifest, out IAddableContainer<Hash, Slice> container)
        {
            // Create a directory for this manifest
            container = null;
            if (!_options.ManifestsFolder.Directories.TryCreate(GetDirectoryNameFor(manifest), out var slicesDirectory))
                return false;

            // Write out the manifest to a file
            if (!slicesDirectory.Files.TryCreate(GetFileNameFor(manifest), out var manifestFile))
                return false;
            if (!manifestFile.TryOpenWrite(out var manifestStream))
                return false;
            using (manifestStream)
                manifestStream.Write(manifest);

            // Create a container object for the slices directory
            return TryCreateContainerFor(slicesDirectory, out container);
        }

        // Set up the function for getting an existing container for hash slices associated with the given manifest
        public bool TryGet(Manifest manifest, out IAddableContainer<Hash, Slice> container)
        {
            // Try getting the directory that is used for this manifest
            container = null;
            if (!_options.ManifestsFolder.Directories.TryGet(GetDirectoryNameFor(manifest), out var slicesDirectory))
                return false;

            // Create a container object for the slices directory
            return TryCreateContainerFor(slicesDirectory, out container);
        }

        // Set up the function for removing everything associated with a manifest
        public bool TryRemove(Manifest manifest) => _options.ManifestsFolder.Directories.TryRemove(GetDirectoryNameFor(manifest));
    }
}

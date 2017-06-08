using System.Collections.Generic;

namespace DistributedStorage.Storage
{
    using System.IO;
    using Common;
    using Encoding;

    public sealed class FileSliceContainerFactory : ISliceContainerFactory
    {
        private readonly DirectoryInfo _baseDirectory;

        public FileSliceContainerFactory(DirectoryInfo baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public IContainer<Hash, Slice> CreateSliceContainer(Manifest forManifest)
        {
            // Create a subdirectory for slices connected to this manifest
            var workingDirectory = _baseDirectory.CreateSubdirectory(forManifest.Id.HashCode.ToHex());

            // Set up the GetKeys function
            IEnumerable<Hash> GetKeys()
            {
                foreach (var file in workingDirectory.EnumerateFiles())
                {
                    if (!file.Name.TryToBytes(out var hashBytes))
                        continue;
                    Hash hash;
                    try
                    {
                        hash = new Hash(hashBytes);
                    }
                    catch
                    {
                        continue;
                    }
                    yield return hash;
                }
            }

            // Set up the TryAdd function
            bool TryAdd(Hash hash, Slice slice)
            {
                try
                {
                    using (var stream = GetFileInfoFor(workingDirectory, hash).Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        stream.Write(slice);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            // Set up the TryGet function
            bool TryGet(Hash hash, out Slice slice)
            {
                try
                {
                    using (var stream = GetFileInfoFor(workingDirectory, hash).Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return stream.TryImmediateRead(out slice);
                    }
                }
                catch
                {
                    slice = null;
                    return false;
                }
            }

            // Set up the TryRemove function
            bool TryRemove(Hash hash)
            {
                var fileInfo = GetFileInfoFor(workingDirectory, hash);
                var exists = fileInfo.Exists;
                if (exists)
                    fileInfo.Delete();
                return exists;
            }

            // Return a new IContainer using the above functions
            return new Container<Hash, Slice>(new Container<Hash, Slice>.Options
            {
                GetKeys = GetKeys,
                TryAdd = TryAdd,
                TryGet = TryGet,
                TryRemove = TryRemove
            });
        }

        /// <summary>
        /// Creates a <see cref="FileInfo"/> that should be used for the given <paramref name="hash"/>
        /// </summary>
        // ReSharper disable once SuggestBaseTypeForParameter
        private static FileInfo GetFileInfoFor(DirectoryInfo workingDirectory, Hash hash) => new FileInfo(Path.Combine(workingDirectory.FullName, hash.HashCode.ToHex()));
    }
}

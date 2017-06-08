namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using Common;
    using Encoding;
    using IFolder = IFactoryContainer<string, IFile>;

    public sealed class FileSliceContainerFactory : ISliceContainerFactory
    {
        private readonly IFactoryContainer<string, IFolder> _baseContainer;

        public FileSliceContainerFactory(IFactoryContainer<string, IFolder> baseContainer)
        {
            _baseContainer = baseContainer;
        }

        public IContainer<Hash, Slice> CreateSliceContainer(Manifest forManifest)
        {
            // Create a subdirectory for slices connected to this manifest
            var workingDirectory = _baseContainer.GetOrCreate(forManifest.Id.HashCode.ToHex());

            // Set up the GetKeys function
            IEnumerable<Hash> GetKeys()
            {
                foreach (var key in workingDirectory.GetKeys())
                {
                    if (!key.TryToBytes(out var hashBytes))
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
                    if (!workingDirectory.TryCreate(HashToString(hash), out var file))
                        return false;
                    if (!file.TryOpenWrite(out var stream))
                        return false;
                    using (stream)
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
                slice = null;
                try
                {
                    if (!workingDirectory.TryGet(HashToString(hash), out var file))
                        return false;
                    if (!file.TryOpenRead(out var stream))
                        return false;
                    using (stream)
                    {
                        return stream.TryImmediateRead(out slice);
                    }
                }
                catch
                {
                    return false;
                }
            }

            // Set up the TryRemove function
            bool TryRemove(Hash hash) => workingDirectory.TryRemove(HashToString(hash));

            // Return a new IContainer using the above functions
            return new Container<Hash, Slice>(new Container<Hash, Slice>.Options
            {
                GetKeys = GetKeys,
                TryAdd = TryAdd,
                TryGet = TryGet,
                TryRemove = TryRemove
            });
        }

        private static string HashToString(Hash hash) => hash.HashCode.ToHex();
    }
}

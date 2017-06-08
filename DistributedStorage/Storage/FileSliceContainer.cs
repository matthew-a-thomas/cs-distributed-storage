namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using Common;
    using Encoding;
    using IFolder = IFactoryContainer<string, IFile>;

    public sealed class FileSliceContainer : IContainer<Hash, Slice>
    {
        private readonly IFolder _workingFolder;

        public FileSliceContainer(IFolder workingFolder)
        {
            _workingFolder = workingFolder;
        }

        public IEnumerable<Hash> GetKeys()
        {
            foreach (var key in _workingFolder.GetKeys())
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

        private static string HashToString(Hash hash) => hash.HashCode.ToHex();
        
        public bool TryAdd(Hash hash, Slice slice)
        {
            try
            {
                if (!_workingFolder.TryCreate(HashToString(hash), out var file))
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
        
        public bool TryGet(Hash hash, out Slice slice)
        {
            slice = null;
            try
            {
                if (!_workingFolder.TryGet(HashToString(hash), out var file))
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
        
        public bool TryRemove(Hash hash) => _workingFolder.TryRemove(HashToString(hash));
    }
}

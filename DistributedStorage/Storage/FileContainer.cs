namespace DistributedStorage.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;

    public sealed class FileContainer<TValue> : IContainer<byte[], TValue>
    {
        private readonly Func<byte[], TValue> _deserializer;
        private readonly Func<TValue, byte[]> _serializer;
        private readonly DirectoryInfo _workingDirectory;

        public FileContainer(
            Func<byte[], TValue> deserializer,
            Func<TValue, byte[]> serializer,
            DirectoryInfo workingDirectory)
        {
            _deserializer = deserializer;
            _serializer = serializer;
            _workingDirectory = workingDirectory;
        }

        private static FileInfo GetFileInfoFor(byte[] key) => new FileInfo(key.ToHex());

        public IEnumerable<byte[]> GetKeys()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in _workingDirectory.EnumerateFiles())
            {
                if (file.Name.TryToBytes(out var hashBytes))
                    yield return hashBytes;
            }
        }

        public bool TryAdd(byte[] key, TValue value)
        {
            try
            {
                using (var stream = GetFileInfoFor(key).Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    var bytes = _serializer(value);
                    stream.Write(bytes);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool TryGet(byte[] key, out TValue value)
        {
            value = default(TValue);
            try
            {
                using (var stream = GetFileInfoFor(key).Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (!stream.TryImmediateRead(out byte[] bytes))
                        return false;
                    value = _deserializer(bytes);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool TryRemove(byte[] key)
        {
            var fileInfo = GetFileInfoFor(key);
            var exists = fileInfo.Exists;
            if (exists)
                fileInfo.Delete();
            return exists;
        }
    }
}

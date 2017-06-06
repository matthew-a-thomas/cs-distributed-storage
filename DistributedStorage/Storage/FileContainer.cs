namespace DistributedStorage.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;

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

        public IEnumerable<byte[]> GetKeys()
        {
            foreach (var file in _workingDirectory.EnumerateFiles())
            {
                if (file.Name)
            }
        }

        public bool TryAdd(byte[] key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(byte[] key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(byte[] key)
        {
            throw new NotImplementedException();
        }
    }
}

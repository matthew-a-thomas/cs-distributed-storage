namespace DistributedStorage.Storage
{
    using System.IO;
    using System.Linq;
    using Common;
    using Encoding;

    public sealed class FileSliceContainerFactory : ISliceContainerFactory
    {
        private readonly DirectoryInfo _workingDirectory;

        public FileSliceContainerFactory(DirectoryInfo workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        public IContainer<Hash, Slice> CreateSliceContainer(Manifest forManifest)
        {
            var fileContainer = new FileContainer<Slice>(
                deserializer: bytes =>
                {
                    using (var stream = new MemoryStream(bytes))
                    {
                        return stream.TryImmediateRead(out Slice slice) ? slice : null;
                    }
                },
                serializer: slice =>
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(slice);
                        return stream.ToArray();
                    }
                },
                workingDirectory: new DirectoryInfo(Path.Combine(_workingDirectory.FullName, forManifest.Id.HashCode.ToHex()))
                );

            return new Container<Hash, Slice>(new Container<Hash, Slice>.Options
            {
                GetKeys = () => fileContainer.GetKeys().Select(Hash.Create),
                TryAdd = (hash, slice) => fileContainer.TryAdd(hash.HashCode, slice.ToBytes())
            });
        }
    }
}

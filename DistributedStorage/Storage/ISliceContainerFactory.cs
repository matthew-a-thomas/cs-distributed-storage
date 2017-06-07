namespace DistributedStorage.Storage
{
    using Common;
    using Encoding;

    public interface ISliceContainerFactory
    {
        IContainer<Hash, Slice> CreateSliceContainer(Manifest forManifest);
    }
}

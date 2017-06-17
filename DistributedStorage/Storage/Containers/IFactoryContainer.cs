namespace DistributedStorage.Storage.Containers
{
    public interface IFactoryContainer<TKey, TValue> : IReadableContainer<TKey, TValue>, IRemovableContainer<TKey>
    {
        bool TryCreate(TKey key, out TValue value);
    }
}

namespace DistributedStorage.Storage
{
    public interface IFactoryContainer<TKey, TValue> : IContainer<TKey, TValue>
    {
        bool TryCreate(TKey key, out TValue value);
    }
}

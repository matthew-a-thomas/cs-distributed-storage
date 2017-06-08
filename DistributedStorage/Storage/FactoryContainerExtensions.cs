namespace DistributedStorage.Storage
{
    public static class FactoryContainerExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IFactoryContainer<TKey, TValue> factoryContainer, TKey key)
        {
            while (true)
            {
                if (factoryContainer.TryGet(key, out var value) || factoryContainer.TryCreate(key, out value))
                    return value;
            }
        }
    }
}

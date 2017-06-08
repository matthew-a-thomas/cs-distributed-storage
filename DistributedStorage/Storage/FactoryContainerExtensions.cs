namespace DistributedStorage.Storage
{
    using System.Collections.Generic;

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

        /// <summary>
        /// Adapts this <see cref="IDictionary{TKey, TValue}"/> into an <see cref="IFactoryContainer{TKey, TValue}"/>
        /// </summary>
        public static IFactoryContainer<TKey, TValue> ToFactoryContainer<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
            where TValue : new()
        {
            bool TryCreate(TKey key, out TValue value)
            {
                value = default(TValue);
                if (dictionary.ContainsKey(key))
                    return false;
                value = new TValue();
                dictionary[key] = value;
                return true;
            }

            return new FactoryContainer<TKey, TValue>(
                container: new Container<TKey, TValue>(new Container<TKey, TValue>.Options
                {
                    GetKeys = () => dictionary.Keys,
                    TryAdd = (key, value) =>
                    {
                        if (dictionary.ContainsKey(key))
                            return false;
                        dictionary[key] = value;
                        return true;
                    },
                    TryGet = dictionary.TryGetValue,
                    TryRemove = dictionary.Remove
                }),
                options: new FactoryContainer<TKey, TValue>.Options
                {
                    TryCreate = TryCreate
                });
        }
    }
}

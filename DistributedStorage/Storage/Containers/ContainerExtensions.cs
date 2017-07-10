namespace DistributedStorage.Storage.Containers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ContainerExtensions
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> GetKeysAndValues<TKey, TValue>(this IReadableContainer<TKey, TValue> container)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var key in container.GetKeys())
                if (container.TryGet(key, out var value))
                    yield return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static TValue GetOrCreate<TKey, TValue>(this IFactoryContainer<TKey, TValue> factoryContainer, TKey key)
        {
            while (true)
            {
                if (factoryContainer.TryGet(key, out var value) || factoryContainer.TryCreate(key, out value))
                    return value;
            }
        }

        public static IEnumerable<TValue> GetValues<TKey, TValue>(this IReadableContainer<TKey, TValue> container) => container.GetKeysAndValues().Select(kvp => kvp.Value);
        
        /// <summary>
        /// Adapts this <see cref="IDictionary{TKey, TValue}"/> into an <see cref="IFactoryContainer{TKey, TValue}"/>
        /// </summary>
        public static IAddableContainer<TKey, TValue> ToContainer<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => new Container<TKey, TValue>(new Container<TKey, TValue>.Options
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
        });
    }
}

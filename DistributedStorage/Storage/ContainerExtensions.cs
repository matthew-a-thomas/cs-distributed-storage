namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ContainerExtensions
    {
        public static IEnumerable<KeyValuePair<TKey, TValue>> GetKeysAndValues<TKey, TValue>(this IContainer<TKey, TValue> container)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var key in container.GetKeys())
                if (container.TryGet(key, out var value))
                    yield return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static IEnumerable<TValue> GetValues<TKey, TValue>(this IContainer<TKey, TValue> container) => container.GetKeysAndValues().Select(kvp => kvp.Value);
    }
}

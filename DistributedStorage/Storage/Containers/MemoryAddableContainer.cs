namespace DistributedStorage.Storage.Containers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public sealed class MemoryAddableContainer<TKey, TValue> : IAddableContainer<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _memory = new ConcurrentDictionary<TKey, TValue>();

        public IEnumerable<TKey> GetKeys() => _memory.Keys;

        public bool TryAdd(TKey key, TValue value) => _memory.TryAdd(key, value);

        public bool TryGet(TKey key, out TValue value) => _memory.TryGetValue(key, out value);

        public bool TryRemove(TKey key) => _memory.TryRemove(key, out _);
    }
}

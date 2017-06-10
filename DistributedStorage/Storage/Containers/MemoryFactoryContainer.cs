namespace DistributedStorage.Storage.Containers
{
    using System;
    using System.Collections.Generic;

    public sealed class MemoryFactoryContainer<TKey, TValue> : IFactoryContainer<TKey, TValue>
    {
        private readonly Func<TValue> _factory;
        private readonly IDictionary<TKey, TValue> _memory = new Dictionary<TKey, TValue>();

        public MemoryFactoryContainer(Func<TValue> factory)
        {
            _factory = factory;
        }

        public IEnumerable<TKey> GetKeys() => _memory.Keys;

        public bool TryGet(TKey key, out TValue value) => _memory.TryGetValue(key, out value);

        public bool TryRemove(TKey key) => _memory.Remove(key);

        public bool TryCreate(TKey key, out TValue value)
        {
            value = default(TValue);
            if (_memory.ContainsKey(key))
                return false;
            _memory[key] = value = _factory();
            return true;
        }
    }
}

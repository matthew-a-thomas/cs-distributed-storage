namespace DistributedStorage.Storage
{
    using System;

    /// <summary>
    /// Something that uses a <see cref="WeakReference{T}"/> to maintain a cache
    /// </summary>
    public sealed class WeakCache<T> : ICache<T> where T : class
    {
        private readonly WeakReference<T> _cache = new WeakReference<T>(null);
        private readonly Func<T> _generateValue;

        public WeakCache(Func<T> generateValue)
        {
            _generateValue = generateValue;
        }

        public T GetValue()
        {
            // Try getting the value from the cache
            if (_cache.TryGetTarget(out var value))
                return value;

            // Cache miss: generate a new one
            value = _generateValue();
            _cache.SetTarget(value);

            return value;
        }
    }
}

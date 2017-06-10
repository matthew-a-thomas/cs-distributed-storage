namespace DistributedStorage.Storage
{
    using System.Collections.Generic;

    /// <summary>
    /// A container that can be read from
    /// </summary>
    public interface IReadableContainer<TKey, TValue>
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> that iterates all keys
        /// </summary>
        IEnumerable<TKey> GetKeys();

        /// <summary>
        /// Tries to get the <paramref name="value"/> for the given <paramref name="key"/>
        /// </summary>
        bool TryGet(TKey key, out TValue value);
    }
}
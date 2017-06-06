namespace DistributedStorage.Storage
{
    using System.Collections.Generic;

    /// <summary>
    /// Something that manages keyed values
    /// </summary>
    public interface IContainer<TKey, TValue>
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> that iterates all keys
        /// </summary>
        IEnumerable<TKey> GetKeys();

        /// <summary>
        /// Adds the <paramref name="value"/> for the given <paramref name="key"/> if that <paramref name="key"/> doesn't already exist
        /// </summary>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// Tries to get the <paramref name="value"/> for the given <paramref name="key"/>
        /// </summary>
        bool TryGet(TKey key, out TValue value);

        /// <summary>
        /// Tries to remove the value for the given <paramref name="key"/>
        /// </summary>
        bool TryRemove(TKey key);
    }
}

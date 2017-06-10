namespace DistributedStorage.Storage
{
    /// <summary>
    /// Something that manages keyed values
    /// </summary>
    public interface IAddableContainer<TKey, TValue> : IRemovableContainer<TKey>, IReadableContainer<TKey, TValue>
    {
        /// <summary>
        /// Adds the <paramref name="value"/> for the given <paramref name="key"/> if that <paramref name="key"/> doesn't already exist
        /// </summary>
        bool TryAdd(TKey key, TValue value);
    }
}

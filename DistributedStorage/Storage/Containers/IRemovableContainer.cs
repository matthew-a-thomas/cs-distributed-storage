namespace DistributedStorage.Storage.Containers
{
    /// <summary>
    /// Something that has things that can be removed
    /// </summary>
    public interface IRemovableContainer<in TKey>
    {
        /// <summary>
        /// Tries to remove the value for the given <paramref name="key"/>
        /// </summary>
        bool TryRemove(TKey key);
    }
}
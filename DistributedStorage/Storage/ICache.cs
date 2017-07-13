namespace DistributedStorage.Storage
{
    /// <summary>
    /// Something which knows how to cache values
    /// </summary>
    public interface ICache<out T>
    {
        /// <summary>
        /// Retrieve the cached value if possible, or a new value
        /// </summary>
        T GetValue();
    }
}

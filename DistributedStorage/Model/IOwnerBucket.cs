namespace DistributedStorage.Model
{
    /// <summary>
    /// A bucket from the perspective of the owner of that bucket
    /// </summary>
    public interface IOwnerBucket
    {
        /// <summary>
        /// Deletes this bucket and all its contents
        /// </summary>
        void Delete();
    }
}

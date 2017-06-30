namespace DistributedStorage.Model
{
    using Common;
    using Encoding;

    /// <summary>
    /// A bucket from the perspective of the <see cref="IPool{TIdentity}"/> that is authorized to manage it
    /// </summary>
    public interface IPoolBucket
    {
        /// <summary>
        /// Adds the given <paramref name="slices"/> to this bucket, associating them with the given <see cref="Manifest"/>
        /// </summary>
        void AddSlices(Manifest forManifest, Slice[] slices);

        /// <summary>
        /// Deletes the <see cref="Slice"/>s having the given <paramref name="hashesToDelete"/> which are associated with the given <see cref="Manifest"/>
        /// </summary>
        void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete);
    }
}

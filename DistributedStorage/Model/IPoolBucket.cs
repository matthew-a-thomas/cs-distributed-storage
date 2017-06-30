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
        [Guid("d11aeaa1-1f4d-4f84-a1ac-d4359d110cc0")]
        void AddSlices(Manifest forManifest, Slice[] slices);

        /// <summary>
        /// Deletes the <see cref="Slice"/>s having the given <paramref name="hashesToDelete"/> which are associated with the given <see cref="Manifest"/>
        /// </summary>
        [Guid("147e6713-83c1-4c24-b002-253770e009b7")]
        void DeleteSlices(Manifest forManifest, Hash[] hashesToDelete);
    }
}

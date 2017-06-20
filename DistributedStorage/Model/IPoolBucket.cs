namespace DistributedStorage.Model
{
    using System.Collections.Generic;
    using Common;
    using Encoding;

    /// <summary>
    /// A bucket from the perspective of the <see cref="IPool"/> that is authorized to manage it
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

        /// <summary>
        /// Enumerates all <see cref="Slice"/> <see cref="Hash"/>es associated with the given <see cref="Manifest"/>
        /// </summary>
        IEnumerable<Hash> GetHashes(Manifest forManifest);

        /// <summary>
        /// Enumerates all <see cref="Manifest"/>s in this bucket
        /// </summary>
        IEnumerable<Manifest> GetManifests();

        /// <summary>
        /// Enumerates all <see cref="Slice"/>s that are associated with the given <see cref="Manifest"/> in this bucket and have the given <paramref name="hashes"/>
        /// </summary>
        IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes);
    }
}

namespace DistributedStorage.Model
{
    using System.Collections.Generic;
    using Common;
    using Encoding;

    /// <summary>
    /// Some storage that is owned by someone and belongs to a pool
    /// </summary>
    public interface IBucket<out TIdentity>
        where TIdentity : IIdentity
    {
        /// <summary>
        /// Gets the current size of the contents of this <see cref="IBucket{TIdentity}"/>
        /// </summary>
        long GetCurrentSize();

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

        /// <summary>
        /// The maximum size that the <see cref="OwnerIdentity"/> desires this <see cref="IBucket{TIdentity}"/> to be
        /// </summary>
        long MaxSize { get; }

        /// <summary>
        /// The identity of the party who owns this <see cref="IBucket{TIdentity}"/>.
        /// This party has the authority to remove this <see cref="IBucket{TIdentity}"/> and owns the associated <see cref="Storage"/>,
        /// but shouldn't be directly adding and removing slices--that's the job of the <see cref="PoolIdentity"/>
        /// </summary>
        TIdentity OwnerIdentity { get; }

        /// <summary>
        /// The identity of the pool to which this <see cref="IBucket{TIdentity}"/> belongs.
        /// This <see cref="PoolIdentity"/> has the authority to add and delete slices from this <see cref="IBucket{TIdentity}"/>
        /// </summary>
        TIdentity PoolIdentity { get; }

        /// <summary>
        /// An identifier for this <see cref="IBucket{TIdentity}"/>
        /// </summary>
        TIdentity SelfIdentity { get; }
    }
}

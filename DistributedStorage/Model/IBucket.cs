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
        [Guid("a0ac87aa-d5b0-481b-8a0e-41caab0ec694")]
        long GetCurrentSize();

        /// <summary>
        /// Enumerates all <see cref="Slice"/> <see cref="Hash"/>es associated with the given <see cref="Manifest"/>
        /// </summary>
        [Guid("3ef5d5e3-f3f6-4464-884a-471c1536212f")]
        IEnumerable<Hash> GetHashes(Manifest forManifest);

        /// <summary>
        /// Enumerates all <see cref="Manifest"/>s in this bucket
        /// </summary>
        [Guid("5682064d-eba5-4e94-9ae6-fba6b6d3b40e")]
        IEnumerable<Manifest> GetManifests();

        /// <summary>
        /// Enumerates all <see cref="Slice"/>s that are associated with the given <see cref="Manifest"/> in this bucket and have the given <paramref name="hashes"/>
        /// </summary>
        [Guid("023ab142-4cde-4c42-b149-6e488894640b")]
        IEnumerable<Slice> GetSlices(Manifest forManifest, Hash[] hashes);

        /// <summary>
        /// The maximum size that the <see cref="OwnerIdentity"/> desires this <see cref="IBucket{TIdentity}"/> to be
        /// </summary>
        [Guid("e5d04f80-290b-44f9-8ec8-1730801b71e4")]
        long MaxSize { get; }

        /// <summary>
        /// The identity of the party who owns this <see cref="IBucket{TIdentity}"/>.
        /// This party has the authority to remove this <see cref="IBucket{TIdentity}"/> and owns the associated <see cref="Storage"/>,
        /// but shouldn't be directly adding and removing slices--that's the job of the <see cref="PoolIdentity"/>
        /// </summary>
        [Guid("32112af0-0e19-45fc-afe9-2008ee630597")]
        TIdentity OwnerIdentity { get; }

        /// <summary>
        /// The identity of the pool to which this <see cref="IBucket{TIdentity}"/> belongs.
        /// This <see cref="PoolIdentity"/> has the authority to add and delete slices from this <see cref="IBucket{TIdentity}"/>
        /// </summary>
        [Guid("bbac79bf-2a63-48bf-a1f1-4b1382917ed6")]
        TIdentity PoolIdentity { get; }

        /// <summary>
        /// An identifier for this <see cref="IBucket{TIdentity}"/>
        /// </summary>
        [Guid("61762277-de74-4a78-8f57-d6db64373723")]
        TIdentity SelfIdentity { get; }
    }
}

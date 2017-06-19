namespace DistributedStorage.Networking.Model
{
    /// <summary>
    /// Some storage that is owned by someone and belongs to a pool
    /// </summary>
    public interface IBucket
    {
        /// <summary>
        /// The identity of the party who owns this <see cref="IBucket"/>.
        /// This party has the authority to remove this <see cref="IBucket"/> and owns the associated <see cref="Storage"/>,
        /// but shouldn't be directly adding and removing slices--that's the job of the <see cref="Pool"/>
        /// </summary>
        IOwner Owner { get; }

        /// <summary>
        /// The identity of the pool to which this <see cref="IBucket"/> belongs.
        /// This <see cref="Pool"/> has the authority to add and delete slices from this <see cref="IBucket"/>
        /// </summary>
        IPool Pool { get; }

        /// <summary>
        /// The maximum size that the <see cref="Owner"/> desires this <see cref="IBucket"/> to be
        /// </summary>
        long Size { get; }
    }
}

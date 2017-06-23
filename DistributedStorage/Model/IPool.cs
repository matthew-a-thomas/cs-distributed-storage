namespace DistributedStorage.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// A managed collection of <see cref="IBucket{TIdentity}"/>s.
    /// <see cref="IBucket{TIdentity}"/>s are added and removed by their <see cref="IOwner"/>s, and this <see cref="IPool{TIdentity}"/> tells the <see cref="IBucket{TIdentity}"/>s what to store
    /// </summary>
    public interface IPool<out TIdentity>
        where TIdentity : IIdentity
    {
        /// <summary>
        /// The <see cref="IBucket{TIdentity}"/>s that belong to this <see cref="IPool{TIdentity}"/>
        /// </summary>
        IEnumerable<IBucket<TIdentity>> Buckets { get; }

        /// <summary>
        /// The identity of this <see cref="IPool{TIdentity}"/>
        /// </summary>
        TIdentity Identity { get; }

        /// <summary>
        /// The policy for determining <see cref="IQuota"/>s
        /// </summary>
        IQuotaPolicy QuotaPolicy { get; }
    }
}

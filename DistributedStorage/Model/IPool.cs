﻿namespace DistributedStorage.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// A managed collection of <see cref="IBucket"/>s.
    /// <see cref="IBucket"/>s are added and removed by their <see cref="IOwner"/>s, and this <see cref="IPool"/> tells the <see cref="IBucket"/>s what to store
    /// </summary>
    public interface IPool
    {
        /// <summary>
        /// The <see cref="IBucket"/>s that belong to this <see cref="IPool"/>
        /// </summary>
        IEnumerable<IBucket> Buckets { get; }

        /// <summary>
        /// The identity of this <see cref="IPool"/>
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// The policy for determining <see cref="IQuota"/>s
        /// </summary>
        IQuotaPolicy QuotaPolicy { get; }
    }
}
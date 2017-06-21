namespace DistributedStorage.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class Pool<TIdentity> : IPool<TIdentity>
        where TIdentity : IIdentity
    {
        private readonly Func<IEnumerable<IBucket<TIdentity>>> _bucketsDelegate;

        public Pool(Func<IEnumerable<IBucket<TIdentity>>> bucketsDelegate, TIdentity identity, IQuotaPolicy quotaPolicy)
        {
            Identity = identity;
            QuotaPolicy = quotaPolicy;
            _bucketsDelegate = bucketsDelegate;
        }

        public IEnumerable<IBucket<TIdentity>> Buckets => _bucketsDelegate();
        public TIdentity Identity { get; }
        public IQuotaPolicy QuotaPolicy { get; }
    }
}

namespace DistributedStorage.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class Pool : IPool
    {
        private readonly Func<IEnumerable<IBucket>> _bucketsDelegate;

        public Pool(Func<IEnumerable<IBucket>> bucketsDelegate, IIdentity identity, IQuotaPolicy quotaPolicy)
        {
            Identity = identity;
            QuotaPolicy = quotaPolicy;
            _bucketsDelegate = bucketsDelegate;
        }

        public IEnumerable<IBucket> Buckets => _bucketsDelegate();
        public IIdentity Identity { get; }
        public IQuotaPolicy QuotaPolicy { get; }
    }
}

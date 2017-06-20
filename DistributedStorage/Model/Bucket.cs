
namespace DistributedStorage.Model
{
    public sealed class Bucket : IBucket
    {
        public Bucket(IIdentity identity, IIdentity owner, IIdentity pool, long size)
        {
            SelfIdentity = identity;
            OwnerIdentity = owner;
            PoolIdentity = pool;
            Size = size;
        }

        public IIdentity SelfIdentity { get; }
        public IIdentity OwnerIdentity { get; }
        public IIdentity PoolIdentity { get; }
        public long Size { get; }
    }
}

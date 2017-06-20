
namespace DistributedStorage.Model
{
    public sealed class Bucket : IBucket
    {
        public Bucket(IOwner owner, IPool pool, long size)
        {
            Owner = owner;
            Pool = pool;
            Size = size;
        }

        public IOwner Owner { get; }
        public IPool Pool { get; }
        public long Size { get; }
    }
}

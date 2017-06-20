namespace DistributedStorage.Model
{
    public sealed class Quota : IQuota
    {
        public Quota(long max, IOwner owner)
        {
            Max = max;
            Owner = owner;
        }

        public long Max { get; }
        public IOwner Owner { get; }
    }
}

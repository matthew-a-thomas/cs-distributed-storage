namespace DistributedStorage.Model
{
    public sealed class Owner : IOwner
    {
        public Owner(IIdentity identity)
        {
            Identity = identity;
        }

        public IIdentity Identity { get; }
    }
}

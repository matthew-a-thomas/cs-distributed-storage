namespace DistributedStorage.Model
{
    using Common;

    public sealed class Identity : IIdentity
    {
        public Identity(Hash claim)
        {
            Claim = claim;
        }

        public Hash Claim { get; }
    }
}

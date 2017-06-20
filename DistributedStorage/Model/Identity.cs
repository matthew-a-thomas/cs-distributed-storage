namespace DistributedStorage.Model
{
    public sealed class Identity : IIdentity
    {
        public Identity(long claim)
        {
            Claim = claim;
        }

        public long Claim { get; }
    }
}

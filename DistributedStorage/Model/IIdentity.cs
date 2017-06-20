namespace DistributedStorage.Model
{
    /// <summary>
    /// A party's identity
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// The claimed identity
        /// </summary>
        long Claim { get; }
    }
}

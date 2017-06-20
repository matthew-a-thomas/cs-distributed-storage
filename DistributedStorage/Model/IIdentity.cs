namespace DistributedStorage.Model
{
    using Common;

    /// <summary>
    /// A party's identity
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// A <see cref="Hash"/> of the claimed identity
        /// </summary>
        Hash Claim { get; }
    }
}

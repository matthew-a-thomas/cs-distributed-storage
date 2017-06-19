namespace DistributedStorage.Networking.Model
{
    /// <summary>
    /// The identity of a party who can manage <see cref="IBucket"/>s
    /// </summary>
    public interface IOwner
    {
        /// <summary>
        /// The <see cref="IOwner"/>'s identity
        /// </summary>
        IIdentity Identity { get; }
    }
}

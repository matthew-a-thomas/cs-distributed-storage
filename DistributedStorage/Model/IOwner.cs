namespace DistributedStorage.Model
{
    /// <summary>
    /// The identity of a party who can manage <see cref="IBucket{TIdentity}"/>s
    /// </summary>
    public interface IOwner
    {
        /// <summary>
        /// The <see cref="IOwner"/>'s identity
        /// </summary>
        IIdentity Identity { get; }
    }
}

namespace DistributedStorage.Networking.Model
{
    /// <summary>
    /// A restriction on the total storage allowed by an <see cref="Owner"/> in a <see cref="IPool"/>
    /// </summary>
    public interface IQuota
    {
        /// <summary>
        /// The maximum storage allowed by the <see cref="Owner"/>
        /// </summary>
        long Max { get; }

        /// <summary>
        /// The <see cref="Owner"/> to whom this <see cref="IQuota"/> applies
        /// </summary>
        IOwner Owner { get; }
    }
}

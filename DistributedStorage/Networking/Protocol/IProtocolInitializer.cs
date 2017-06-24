namespace DistributedStorage.Networking.Protocol
{
    /// <summary>
    /// Something which knows how to set up an <see cref="IProtocol"/> with an instance of <typeparamref name="T"/>
    /// </summary>
    public interface IProtocolInitializer<in T>
    {
        /// <summary>
        /// Set up the given <paramref name="protocol"/> with the given instance of <see cref="T"/>
        /// </summary>
        bool TrySetup(IProtocol protocol, T with);
    }
}

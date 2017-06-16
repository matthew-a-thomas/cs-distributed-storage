namespace DistributedStorage.Networking.Protocol
{
    using System.Threading.Tasks;

    /// <summary>
    /// Something that knows how to talk with someone else
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// Asynchronously makes a request to the other party, returning their response when it is ready
        /// </summary>
        Task<byte[]> MakeRequestAsync(string signature, byte[] parameter);

        /// <summary>
        /// Tries to register the given <paramref name="handler"/> to process requests having the given <paramref name="signature"/>
        /// </summary>
        bool TryRegister(string signature, IHandler<byte[], byte[]> handler);

        /// <summary>
        /// Tries to unregister the handler associated with the given <paramref name="signature"/> so that messages with this <paramref name="signature"/> will be ignored
        /// </summary>
        bool TryUnregister(string signature);
    }
}

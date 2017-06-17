namespace DistributedStorage.Networking.Security
{
    /// <summary>
    /// A source of entropy
    /// </summary>
    public interface IEntropy
    {
        /// <summary>
        /// Creates an array of <paramref name="size"/> random bytes
        /// </summary>
        byte[] CreateNonce(int size);
    }
}

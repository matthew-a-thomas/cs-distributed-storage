namespace DistributedStorage.Security
{
    /// <summary>
    /// An implementation of <see cref="IEntropy"/>
    /// </summary>
    public sealed class Entropy : IEntropy
    {
        /// <summary>
        /// Creates a cryptographically-secure array of random bytes
        /// </summary>
        public byte[] CreateNonce(int size) => Crypto.CreateNonce(size);
    }
}

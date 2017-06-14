namespace DistributedStorage.Networking.Security
{
    using System;

    /// <summary>
    /// An injected implementation of <see cref="IEntropy"/>
    /// </summary>
    public sealed class Entropy : IEntropy
    {
        /// <summary>
        /// The delegate to use for <see cref="CreateNonce(int)"/>
        /// </summary>
        private readonly Func<int, byte[]> _createNonceDelegate;

        /// <summary>
        /// Creates a new <see cref="Entropy"/>, which uses the given <paramref name="createNonceDelegate"/> for <see cref="CreateNonce(int)"/>
        /// </summary>
        public Entropy(Func<int, byte[]> createNonceDelegate)
        {
            _createNonceDelegate = createNonceDelegate;
        }

        /// <summary>
        /// Returns whatever the delegate returns that was passed in through the constructor
        /// </summary>
        public byte[] CreateNonce(int size) => _createNonceDelegate(size);
    }
}

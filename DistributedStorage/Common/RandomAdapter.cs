namespace DistributedStorage.Common
{
    using System;
    using Networking.Security;

    /// <summary>
    /// Adapts a <see cref="Random"/> into an <see cref="IEntropy"/>
    /// </summary>
    public sealed class RandomAdapter : IEntropy
    {
        /// <summary>
        /// The adapted <see cref="Random"/>
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Creates a new <see cref="RandomAdapter"/> that adapts the given <see cref="Random"/> into an <see cref="IEntropy"/>
        /// </summary>
        public RandomAdapter(Random random)
        {
            _random = random;
        }
        
        /// <summary>
        /// Creates a random array of bytes of the given <paramref name="size"/>
        /// </summary>
        public byte[] CreateNonce(int size)
        {
            var buffer = new byte[size];
            _random.NextBytes(buffer);
            return buffer;
        }
    }
}

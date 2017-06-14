namespace DistributedStorage.Networking.Security
{
    using System;

    public static class EntropyExtensions
    {
        /// <summary>
        /// Creates a random <see cref="int"/> from this <see cref="IEntropy"/>
        /// </summary>
        public static int NextInteger(this IEntropy entropy) => BitConverter.ToInt32(entropy.CreateNonce(4), 0);
    }
}

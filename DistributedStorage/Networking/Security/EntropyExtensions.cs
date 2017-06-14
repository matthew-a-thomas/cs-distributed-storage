namespace DistributedStorage.Networking.Security
{
    using System;

    public static class EntropyExtensions
    {
        /// <summary>
        /// Generates a random bit from this <see cref="IEntropy"/>
        /// </summary>
        public static bool NextBit(this IEntropy entropy) => entropy.NextInteger(0, 2) == 1;

        /// <summary>
        /// Creates a random <see cref="int"/> from this <see cref="IEntropy"/>
        /// </summary>
        public static int NextInteger(this IEntropy entropy) => BitConverter.ToInt32(entropy.CreateNonce(4), 0);

        /// <summary>
        /// Returns a number between <paramref name="fromInclusive"/> and <paramref name="toExclusive"/>
        /// </summary>
        public static int NextInteger(this IEntropy entropy, int fromInclusive, int toExclusive) => entropy.NextInteger() % (toExclusive - fromInclusive) + fromInclusive;
    }
}

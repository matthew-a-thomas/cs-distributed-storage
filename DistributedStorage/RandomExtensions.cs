namespace DistributedStorage
{
    internal static class RandomExtensions
    {
        /// <summary>
        /// Generates a random bit from this <see cref="IRandom"/>
        /// </summary>
        public static bool NextBit(this IRandom random) => random.Next(0, 2) == 1;
    }
}

namespace DistributedStorage
{
    using System;

    /// <summary>
    /// Adapts a <see cref="Random"/> into an <see cref="IRandom"/>
    /// </summary>
    public sealed class RandomAdapter : IRandom
    {
        /// <summary>
        /// The adapted <see cref="Random"/>
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Creates a new <see cref="IRandom"/> that adapts the given <see cref="Random"/>
        /// </summary>
        public RandomAdapter(Random random)
        {
            _random = random;
        }

        /// <summary>
        /// The same as invoking <see cref="Random.Next(int, int)"/>
        /// </summary>
        public int Next(int fromInclusive, int toExclusive) => _random.Next(fromInclusive, toExclusive);
    }
}

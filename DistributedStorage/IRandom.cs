namespace DistributedStorage
{
    /// <summary>
    /// Something that generates random numbers
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Returns a number between <paramref name="fromInclusive"/> and <paramref name="toExclusive"/>
        /// </summary>
        int Next(int fromInclusive, int toExclusive);
    }
}

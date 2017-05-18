namespace DistributedStorage
{
    using System.Collections.Generic;

    /// <summary>
    /// Something that creates <see cref="IGenerator"/>s from source data
    /// </summary>
    public interface IGeneratorFactory
    {
        /// <summary>
        /// Creates an <see cref="IGenerator"/> for the given <paramref name="source"/> data
        /// </summary>
        IGenerator CreateGeneratorFor(IReadOnlyList<byte[]> source);
    }
}

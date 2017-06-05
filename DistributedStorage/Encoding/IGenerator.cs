namespace DistributedStorage.Encoding
{
    using Common;

    /// <summary>
    /// Generates <see cref="Slice"/>s from source data
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generates a new <see cref="Slice"/>
        /// </summary>
        Slice Next();
    }
}

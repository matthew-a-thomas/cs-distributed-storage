namespace DistributedStorage.Common
{
    /// <summary>
    /// Something that can convert from one type to another
    /// </summary>
    public interface IConverter<in TFrom, out TTo>
    {
        /// <summary>
        /// Convert the given thing
        /// </summary>
        TTo Convert(TFrom from);
    }
}

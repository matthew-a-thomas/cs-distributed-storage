namespace DistributedStorage.Common
{
    /// <summary>
    /// Something that can convert from one type to another
    /// </summary>
    public interface IConverter<in TFrom, TTo>
    {
        /// <summary>
        /// Convert the given thing
        /// </summary>
        bool TryConvert(TFrom from, out TTo to);
    }
}

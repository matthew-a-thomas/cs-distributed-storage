namespace AspNet.Models
{
    using DistributedStorage.Encoding;

    public static class SliceExtensions
    {
        public static Slice ToSlice(this DistributedStorage.Encoding.Slice slice) => new Slice(
                coefficients: (bool[]) slice.Coefficients.Clone(),
                id: slice.ComputeHash().ToString(),
                data: (byte[]) slice.EncodingSymbol.Clone()
            );
    }
}

namespace DistributedStorage.Encoding
{
    using System.IO;
    using Common;

    /// <summary>
    /// Extension methods for a <see cref="Slice"/> in the context of serialization
    /// </summary>
    public static class SliceExtensions
    {
        /// <summary>
        /// Computes the <see cref="Hash"/> for this <see cref="Slice"/>
        /// </summary>
        public static Hash ComputeHash(this Slice slice)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(slice);
                var bytes = memoryStream.ToArray();
                var hash = Hash.Create(bytes);
                return hash;
            }
        }

        /// <summary>
        /// Deserializes a <see cref="Slice"/> from the given <paramref name="stream"/>
        /// </summary>
        public static bool TryRead(this Stream stream, out Slice slice)
        {
            slice = null;
            if (!stream.TryRead(out int numCoefficients))
                return false;
            if (!stream.TryRead(out byte[] coefficientBytes))
                return false;
            var coefficients = coefficientBytes.AsBits(numCoefficients);
            if (!stream.TryRead(out byte[] encodingSymbol))
                return false;

            slice = new Slice
            {
                Coefficients = coefficients,
                EncodingSymbol = encodingSymbol
            };
            return true;
        }

        /// <summary>
        /// Serializes this <see cref="Slice"/> into the given <paramref name="stream"/>
        /// </summary>
        public static void Write(this Stream stream, Slice slice)
        {
            stream.Write(slice.Coefficients.Length);
            stream.Write(slice.Coefficients.AsBytes());
            stream.Write(slice.EncodingSymbol);
        }
    }
}

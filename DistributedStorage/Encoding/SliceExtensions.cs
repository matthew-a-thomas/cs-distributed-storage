namespace DistributedStorage.Encoding
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Common;

    /// <summary>
    /// Extension methods for a <see cref="Slice"/> in the context of serialization
    /// </summary>
    public static class SliceExtensions
    {
        /// <summary>
        /// Deserializes a <see cref="Slice"/> from the given <paramref name="stream"/>
        /// </summary>
        public static bool TryReadSlice(this Stream stream, TimeSpan timeout, out Slice slice)
        {
            slice = null;
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingRead7BitEncodedInt(timeout, out var numCoefficients))
                return false;
            var numCoefficientBytes = numCoefficients / 8 + (numCoefficients % 8 == 0 ? 0 : 1);
            if (!stream.TryBlockingRead(numCoefficientBytes, timeout - start.Elapsed, out var coefficientBytes))
                return false;
            var coefficients = coefficientBytes.AsBits(numCoefficients);
            if (!stream.TryBlockingReadChunk(timeout - start.Elapsed, out var encodingSymbol))
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
        public static void WriteSlice(this Stream stream, Slice slice)
        {
            stream.Write7BitEncodedInt(slice.Coefficients.Length);
            stream.Write(slice.Coefficients.AsBytes());
            stream.WriteChunk(slice.EncodingSymbol);
        }
    }
}

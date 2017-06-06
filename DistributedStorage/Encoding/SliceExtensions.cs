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
        /// Computes the <see cref="Hash"/> of this <see cref="Slice"/>
        /// </summary>
        public static Hash ComputeHash(this Slice slice) => Hash.Create(slice.EncodingSymbol);

        /// <summary>
        /// Deserializes a <see cref="Slice"/> from the given <paramref name="stream"/>
        /// </summary>
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out Slice slice)
        {
            slice = null;
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingRead(timeout, out int numCoefficients))
                return false;
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out byte[] coefficientBytes))
                return false;
            var coefficients = coefficientBytes.AsBits(numCoefficients);
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out byte[] encodingSymbol))
                return false;

            slice = new Slice
            {
                Coefficients = coefficients,
                EncodingSymbol = encodingSymbol
            };
            return true;
        }

        /// <summary>
        /// Deserializes a <see cref="Slice"/> from the given <paramref name="stream"/>
        /// </summary>
        public static bool TryImmediateRead(this Stream stream, out Slice slice)
        {
            slice = null;
            if (!stream.TryImmediateRead(out int numCoefficients))
                return false;
            if (!stream.TryImmediateRead(out byte[] coefficientBytes))
                return false;
            var coefficients = coefficientBytes.AsBits(numCoefficients);
            if (!stream.TryImmediateRead(out byte[] encodingSymbol))
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

namespace DistributedStorage.Serialization
{
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extension methods for a <see cref="Slice"/> in the context of serialization
    /// </summary>
    public static class SliceExtensions
    {
        /// <summary>
        /// Deserializes a <see cref="Slice"/> from the given <paramref name="stream"/>
        /// </summary>
        public static Slice GetSlice(this Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var numCoefficients = reader.ReadUInt16();
                var numCoefficientBytes = numCoefficients / 8 + (numCoefficients % 8 == 0 ? 0 : 1);
                var coefficientBytes = reader.ReadBytes(numCoefficientBytes);
                var coefficients = coefficientBytes.AsBits(numCoefficients);
                var encodingSymbolLength = reader.ReadInt32();
                var encodingSymbol = reader.ReadBytes(encodingSymbolLength);
                return new Slice
                {
                    Coefficients = coefficients,
                    EncodingSymbol = encodingSymbol
                };
            }
        }

        /// <summary>
        /// Serializes this <see cref="Slice"/> into the given <paramref name="stream"/>
        /// </summary>
        public static void SerializeTo(this Slice slice, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write((ushort)slice.Coefficients.Length); // Write out how many coefficients there are. The number of bytes written will be ceil(Coefficients.Length / 8)
                writer.Write(slice.Coefficients.AsBytes().ToArray()); // Write out the coefficients as a compacted byte array
                writer.Write(slice.EncodingSymbol.Length); // Write out how many bytes are in the encoding symbol
                writer.Write(slice.EncodingSymbol); // Write out the encoding symbol
            }
        }
    }
}

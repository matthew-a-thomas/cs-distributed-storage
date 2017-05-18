namespace DistributedStorage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for primitive type arrays
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Expands this array of bytes into an array of the given number of bits.
        /// It's necessary to know how many bits there are because a byte can hold from 1 to 8 bits
        /// </summary>
        public static bool[] AsBits(this byte[] bytes, int numBits)
        {
            var result = new bool[numBits];
            for (var i = 0; i < numBits; ++i)
            {
                var b = bytes[i / 8];
                var shift = 7 - i % 8;
                result[i] = ((b >> shift) & 0x01) == 1;
            }
            return result;
        }

        /// <summary>
        /// Compacts this array of bits into an array of bytes
        /// </summary>
        public static byte[] AsBytes(this bool[] bits)
        {
            var result = new byte[bits.Length / 8 + (bits.Length % 8 != 0 ? 1 : 0)];
            for (var i = 0; i < bits.Length; ++i)
            {
                if (!bits[i])
                    continue;
                var byteIndex = i / 8;
                var shift = 7 - i % 8;
                result[byteIndex] |= (byte)(0x1 << shift);
            }
            return result;
        }
        
        /// <summary>
        /// Splits the given <paramref name="data"/> into the given <paramref name="numParts"/>
        /// </summary>
        public static IReadOnlyList<byte[]> SplitInto(this byte[] data, int numParts)
        {
            var slices = new byte[numParts][];
            var sliceSize = (int)Math.Ceiling(data.Length / (double)numParts);
            for (var slice = 0; slice < numParts; ++slice)
            {
                var startingIndex = slice * sliceSize;
                var length = Math.Min(sliceSize, data.Length - startingIndex);
                slices[slice] = new byte[sliceSize];
                Array.Copy(data, startingIndex, slices[slice], 0, length);
            }
            return slices;
        }

        /// <summary>
        /// Swaps the thing at <paramref name="from"/> with the thing at <paramref name="to"/>
        /// </summary>
        public static void Swap<T>(this IList<T> list, int from, int to)
        {
            var temp = list[from];
            list[from] = list[to];
            list[to] = temp;
        }
        
        /// <summary>
        /// Modifies this byte array by XOR'ing all the bytes with the given array
        /// </summary>
        public static void Xor(this byte[] array, byte[] with)
        {
            if (array.Length != with.Length)
                throw new ArgumentException("The arrays must have the same length");
            for (var i = 0; i < array.Length; ++i)
                array[i] ^= with[i];
        }

        /// <summary>
        /// Modifies this boolean array by XOR'ing all the bits with the given array
        /// </summary>
        public static void Xor(this bool[] array, bool[] with)
        {
            if (array.Length != with.Length)
                throw new ArgumentException("The arrays must have the same length");
            for (var i = 0; i < array.Length; ++i)
                array[i] ^= with[i];
        }
    }
}

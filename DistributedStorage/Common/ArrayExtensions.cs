namespace DistributedStorage.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Storage;
    using File = Storage.File;

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
        /// Combines the given list of byte arrays into one
        /// </summary>
        public static byte[] Combine(this IReadOnlyList<byte[]> chunks)
        {
            var result = new byte[chunks.Select(chunk => chunk.Length).Sum()];
            var index = 0;
            foreach (var chunk in chunks)
            {
                chunk.CopyTo(result, index);
                index += chunk.Length;
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
        /// Converts a hex string to a byte array
        /// </summary>
        /// <remarks>
        /// Pulled from https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array#comment55025204_321404
        /// </remarks>
        public static byte[] ToBytes(this string hex) => Enumerable.Range(0, hex.Length / 2).Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16)).ToArray();

        /// <summary>
        /// Returns the hexadecimal equivalent of the given data
        /// </summary>
        public static string ToHex(this byte[] data) => BitConverter.ToString(data).Replace("-","").ToLower();

        /// <summary>
        /// Wraps this byte array in an instance of <see cref="IFile"/>.
        /// Note you won't be able to write beyond the bounds of this byte array
        /// </summary>
        public static IFile ToFile(this byte[] data)
        {
            bool TryOpen(out Stream stream)
            {
                stream = new MemoryStream(data);
                return true;
            }

            return new File(new File.Options
            {
                TryOpenRead = TryOpen,
                TryOpenWrite = TryOpen
            });
        }

        /// <summary>
        /// Tries to convert the hex string to a byte array
        /// </summary>
        public static bool TryToBytes(this string hex, out byte[] bytes)
        {
            try
            {
                bytes = hex.ToBytes();
                return true;
            }
            catch
            {
                bytes = null;
                return false;
            }
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

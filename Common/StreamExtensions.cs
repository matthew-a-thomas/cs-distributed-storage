namespace Common
{
    using System;
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Reads in the given number of bytes from this stream
        /// </summary>
        public static byte[] Read(this Stream stream, int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        /// <summary>
        /// Reads in the next chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// What's returned is the data after the integer
        /// </summary>
        public static byte[] ReadChunk(this Stream stream)
        {
            var length = BitConverter.ToInt32(stream.Read(4), 0);
            var data = stream.Read(length);
            return data;
        }

        /// <summary>
        /// Writes the given data out to this stream
        /// </summary>
        public static void Write(this Stream stream, byte[] data) => stream.Write(data, 0, data.Length);

        /// <summary>
        /// Writes out a chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// Both of those things are written
        /// </summary>
        public static void WriteChunk(this Stream stream, byte[] data)
        {
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);
        }
    }
}

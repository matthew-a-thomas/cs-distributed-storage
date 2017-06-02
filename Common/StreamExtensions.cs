namespace Common
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Reads in the given number of bytes from this stream, blocking until that many bytes are available.
        /// Note this is different than the behavior of <see cref="Stream.Read"/>, which might return fewer bytes than requested.
        /// Also note that a <see cref="TimeoutException"/> is thrown if the given <paramref name="timeout"/> is exceeded.
        /// </summary>
        public static byte[] BlockingRead(this Stream stream, int length, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new byte[length];
            var index = 0;
            var buffer = new byte[length];
            while (index < length)
            {
                if (stopwatch.Elapsed > timeout)
                    throw new TimeoutException();
                var numBytesRead = stream.Read(buffer, 0, length);
                for (var i = 0; i < numBytesRead && index < length; ++i)
                {
                    result[index++] = buffer[i];
                }
                length -= numBytesRead;
            }
            return result;
        }

        /// <summary>
        /// Reads in the next chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// What's returned is the data after the integer
        /// </summary>
        public static byte[] BlockingReadChunk(this Stream stream, TimeSpan timeout)
        {
            var start = Stopwatch.StartNew();
            var length = BitConverter.ToInt32(stream.BlockingRead(4, timeout), 0);
            var data = stream.BlockingRead(length, timeout - start.Elapsed);
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

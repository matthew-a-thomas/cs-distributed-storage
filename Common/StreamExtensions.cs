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
        public static bool TryBlockingRead(this Stream stream, int length, TimeSpan timeout, out byte[] data)
        {
            data = null;
            var stopwatch = Stopwatch.StartNew();
            var result = new byte[length];
            var index = 0;
            var buffer = new byte[length];
            while (index < length)
            {
                if (stopwatch.Elapsed > timeout)
                    return false;
                var numBytesRead = stream.Read(buffer, 0, length);
                for (var i = 0; i < numBytesRead && index < length; ++i)
                {
                    result[index++] = buffer[i];
                }
                length -= numBytesRead;
            }
            data = result;
            return true;
        }

        /// <summary>
        /// Reads in the next chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// What's returned is the data after the integer
        /// </summary>
        public static bool TryBlockingReadChunk(this Stream stream, TimeSpan timeout, out byte[] data)
        {
            data = null;
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingRead(4, timeout, out var lengthBytes))
                return false;
            var length = BitConverter.ToInt32(lengthBytes, 0);
            return stream.TryBlockingRead(length, timeout - start.Elapsed, out data);
        }

        /// <summary>
        /// Tries to read the given number of bytes from this <see cref="Stream"/>.
        /// If that number of bytes is not immediately returned from a call to <see cref="Stream.Read(byte[], int, int)"/>, then false is returned.
        /// </summary>
        public static bool TryRead(this Stream stream, int length, out byte[] data)
        {
            var buffer = new byte[length];
            var numBytesRead = stream.Read(buffer, 0, length);
            if (numBytesRead == length)
            {
                data = buffer;
                return true;
            }
            data = null;
            return false;
        }

        /// <summary>
        /// Tries to read the next chunk of data from this <see cref="Stream"/>.
        /// If an entire chunk is not immediately available, then false is returned.
        /// </summary>
        public static bool TryReadChunk(this Stream stream, out byte[] data)
        {
            if (stream.TryRead(4, out var lengthBytes) && stream.TryRead(BitConverter.ToInt32(lengthBytes, 0), out data))
                return true;
            data = null;
            return false;
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

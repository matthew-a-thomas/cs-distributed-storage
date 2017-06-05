namespace DistributedStorage.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Repeatedly attempts to read a single byte from the given <paramref name="stream"/> until either a byte is available or until the <paramref name="timeout"/> is elapsed.
        /// Returns true if a byte was read.
        /// </summary>
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out byte b)
        {
            b = 0;
            var d = -1;
            var start = Stopwatch.StartNew();
            while (d < 0)
            {
                if (start.Elapsed > timeout)
                    return false;
                d = stream.ReadByte();
            }
            b = (byte)d;
            return true;
        }
        
        /// <summary>
        /// Reads in the <paramref name="number"/> that was written using <see cref="Write(System.IO.Stream,int)"/>
        /// </summary>
        /// <remarks>
        /// Inspired by https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs,f30b8b6e8ca06e0f,references
        /// </remarks>
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out int number)
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            number = 0;
            var shift = 0;
            byte b;
            var start = Stopwatch.StartNew();
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    return false;

                if (!stream.TryBlockingRead(timeout - start.Elapsed, out b))
                    return false;
                number |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return true;
        }

        /// <summary>
        /// Reads in the next chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// What's returned is the data after the integer
        /// </summary>
        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out byte[] data)
        {
            data = null;
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingRead(timeout, out int length))
                return false;
            
            var result = new byte[length];
            var index = 0;
            var buffer = new byte[length];
            while (index < length)
            {
                if (start.Elapsed > timeout)
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
        /// Tries to read out a single byte from this <see cref="Stream"/>.
        /// Returns false if one is not immediately available
        /// </summary>
        public static bool TryImmediateRead(this Stream stream, out byte b)
        {
            var d = stream.ReadByte();
            b = (byte)d;
            return d >= 0;
        }
        
        /// <summary>
        /// Reads in the <paramref name="number"/> that was written using <see cref="Write(System.IO.Stream,int)"/>
        /// </summary>
        /// <remarks>
        /// Inspired by https://referencesource.microsoft.com/#mscorlib/system/io/binaryreader.cs,f30b8b6e8ca06e0f,references
        /// </remarks>
        public static bool TryImmediateRead(this Stream stream, out int number)
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            number = 0;
            var shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    return false;

                if (!stream.TryImmediateRead(out b))
                    return false;
                number |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return true;
        }

        /// <summary>
        /// Tries to read the next chunk of data from this <see cref="Stream"/>.
        /// If an entire chunk is not immediately available, then false is returned.
        /// </summary>
        public static bool TryImmediateRead(this Stream stream, out byte[] data)
        {
            data = null;
            if (!stream.TryImmediateRead(out int length))
                return false;

            var buffer = new byte[length];
            var numBytesRead = stream.Read(buffer, 0, length);
            if (numBytesRead != length)
                return false;
            data = buffer;
            return true;
        }
        
        /// <summary>
        /// Writes out the given <paramref name="number"/> in only as many bytes as are needed
        /// </summary>
        /// <remarks>
        /// Inspired by https://referencesource.microsoft.com/#mscorlib/system/io/binarywriter.cs,2daa1d14ff1877bd,references
        /// and by https://en.wikipedia.org/wiki/Variable-length_quantity
        /// </remarks>
        public static void Write(this Stream stream, int number)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            var v = (uint)number;   // support negative numbers
            while (v >= 0x80)
            {
                stream.WriteByte((byte)(v | 0x80));
                v >>= 7;
            }
            stream.WriteByte((byte)v);
        }

        /// <summary>
        /// Writes out a chunk of data.
        /// A chunk of data is defined as an integer specifying how many bytes follow, followed by that many bytes.
        /// Both of those things are written
        /// </summary>
        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data.Length);
            stream.Write(data, 0, data.Length);
        }
    }
}

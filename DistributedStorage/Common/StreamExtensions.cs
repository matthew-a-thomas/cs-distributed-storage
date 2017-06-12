namespace DistributedStorage.Common
{
    using System.IO;

    public static class StreamExtensions
    {
        /// <summary>
        /// Tries to read out a single byte from this <see cref="Stream"/>.
        /// Returns false if one is not immediately available
        /// </summary>
        public static bool TryRead(this Stream stream, out byte b)
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
        public static bool TryRead(this Stream stream, out int number)
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

                if (!stream.TryRead(out b))
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
        public static bool TryRead(this Stream stream, out byte[] data)
        {
            data = null;
            if (!stream.TryRead(out int length))
                return false;

            var buffer = new byte[length];
            var numBytesRead = stream.Read(buffer, 0, length);
            if (numBytesRead != length)
                return false;
            data = buffer;
            return true;
        }

        /// <summary>
        /// Tries to read a string out of this <see cref="Stream"/>
        /// </summary>
        public static bool TryRead(this Stream stream, out string message)
        {
            message = null;
            if (!stream.TryRead(out byte[] bytes))
                return false;
            message = System.Text.Encoding.UTF8.GetString(bytes);
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
        /// Writes the given byte out to this <see cref="Stream"/> by itself
        /// </summary>
        public static void Write(this Stream stream, byte b) => stream.WriteByte(b);

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

        /// <summary>
        /// Writes the given <paramref name="message"/> out to this <see cref="Stream"/>
        /// </summary>
        public static void Write(this Stream stream, string message) => stream.Write(System.Text.Encoding.UTF8.GetBytes(message));
    }
}

namespace Security
{
    using System;
    using System.IO;

    public static class StreamExtensions
    {
        public static byte[] Read(this Stream stream, int length)
        {
            var buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return buffer;
        }

        public static byte[] ReadChunk(this Stream stream)
        {
            var length = BitConverter.ToInt32(stream.Read(4), 0);
            var data = stream.Read(length);
            return data;
        }

        public static void Write(this Stream stream, byte[] data) => stream.Write(data, 0, data.Length);

        public static void WriteChunk(this Stream stream, byte[] data)
        {
            stream.Write(BitConverter.GetBytes(data.Length));
            stream.Write(data);
        }
    }
}

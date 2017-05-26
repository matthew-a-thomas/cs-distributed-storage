namespace Security
{
    using System.IO;
    using System.Security.Cryptography;

    internal static class RsaParametersExtensions
    {
        public static byte[] ToBytes(this RSAParameters key)
        {
            using (var buffer = new MemoryStream())
            {
                buffer.WriteChunk(key.Exponent);
                buffer.WriteChunk(key.Modulus);
                return buffer.ToArray();
            }
        }
    }
}

namespace Security
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    internal static class ArrayExtensions
    {
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

        public static RSAParameters ToRsaPublicKey(this byte[] data)
        {
            using (var buffer = new MemoryStream(data))
            {
                var exponent = buffer.ReadChunk();
                var modulus = buffer.ReadChunk();
                return new RSAParameters
                {
                    Exponent = exponent,
                    Modulus = modulus
                };
            }
        }
    }
}

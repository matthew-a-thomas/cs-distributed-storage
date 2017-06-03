namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    internal static class RsaParametersExtensions
    {
        public static byte[] ComputeHashCode(this RSAParameters key)
        {
            var bytes = key.ToBytes();
            using (var hasher = Crypto.CreateHashAlgorithm())
                return hasher.ComputeHash(bytes);
        }

        public static RSA CreateRsa(this RSAParameters key)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(key);
            return rsa;
        }

        public static int GetKeySize(this RSAParameters key) => key.Modulus.Length;

        public static bool TryReadRsaKey(this Stream stream, TimeSpan timeout, out RSAParameters key)
        {
            key = default(RSAParameters);
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingReadChunk(timeout, out var remoteExponent))
                return false;
            if (!stream.TryBlockingReadChunk(timeout - start.Elapsed, out var remoteModulus))
                return false;
            key = new RSAParameters
            {
                Exponent = remoteExponent,
                Modulus = remoteModulus
            };
            return true;
        }

        public static byte[] ToBytes(this RSAParameters key)
        {
            using (var buffer = new MemoryStream())
            {
                buffer.WritePublicKey(key);
                return buffer.ToArray();
            }
        }

        public static bool TryToRsaPublicKey(this byte[] data, out RSAParameters key)
        {
            key = default(RSAParameters);
            using (var buffer = new MemoryStream(data))
            {
                if (!buffer.TryReadChunk(out var exponent))
                    return false;
                if (!buffer.TryReadChunk(out var modulus))
                    return false;
                key = new RSAParameters
                {
                    Exponent = exponent,
                    Modulus = modulus
                };
                return true;
            }
        }

        public static void WritePublicKey(this Stream stream, RSAParameters key)
        {
            stream.WriteChunk(key.Exponent);
            stream.WriteChunk(key.Modulus);
        }
    }
}

namespace Security
{
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

        public static RSAParameters ReadPublicKey(this Stream stream)
        {
            var remoteExponent = stream.BlockingReadChunk();
            var remoteModulus = stream.BlockingReadChunk();
            var publicKey = new RSAParameters
            {
                Exponent = remoteExponent,
                Modulus = remoteModulus
            };
            return publicKey;
        }

        public static byte[] ToBytes(this RSAParameters key)
        {
            using (var buffer = new MemoryStream())
            {
                buffer.WritePublicKey(key);
                return buffer.ToArray();
            }
        }

        public static RSAParameters ToRsaPublicKey(this byte[] data)
        {
            using (var buffer = new MemoryStream(data))
            {
                var exponent = buffer.BlockingReadChunk();
                var modulus = buffer.BlockingReadChunk();
                return new RSAParameters
                {
                    Exponent = exponent,
                    Modulus = modulus
                };
            }
        }

        public static void WritePublicKey(this Stream stream, RSAParameters key)
        {
            stream.WriteChunk(key.Exponent);
            stream.WriteChunk(key.Modulus);
        }
    }
}

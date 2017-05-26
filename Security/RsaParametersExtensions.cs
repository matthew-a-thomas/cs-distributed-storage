namespace Security
{
    using System.IO;
    using System.Security.Cryptography;

    internal static class RsaParametersExtensions
    {
        public static byte[] ComputeHashCode(this RSAParameters key)
        {
            var bytes = key.ToBytes();
            using (var hasher = Crypto.CreateHashAlgorithm())
                return hasher.ComputeHash(bytes);
        }

        public static RSAParameters ReadPublicKey(this Stream stream)
        {
            var remoteExponent = stream.ReadChunk();
            var remoteModulus = stream.ReadChunk();
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
                buffer.WriteChunk(key.Exponent);
                buffer.WriteChunk(key.Modulus);
                return buffer.ToArray();
            }
        }
    }
}

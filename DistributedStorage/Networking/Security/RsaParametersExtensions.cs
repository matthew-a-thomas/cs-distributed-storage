namespace DistributedStorage.Networking.Security
{
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    public static class RsaParametersExtensions
    {
        public static Hash ToHash(this RSAParameters key)
        {
            var bytes = key.ToBytes();
            return Hash.Create(bytes);
        }

        public static RSA CreateRsa(this RSAParameters key)
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(key);
            return rsa;
        }
        
        public static bool TryRead(this Stream stream, out RSAParameters key)
        {
            key = default(RSAParameters);
            if (!stream.TryRead(out byte[] remoteExponent))
                return false;
            if (!stream.TryRead(out byte[] remoteModulus))
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
                buffer.Write(key);
                return buffer.ToArray();
            }
        }
        
        public static void Write(this Stream stream, RSAParameters key)
        {
            stream.Write(key.Exponent);
            stream.Write(key.Modulus);
        }
    }
}

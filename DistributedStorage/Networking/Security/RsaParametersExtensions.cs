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
            if (!stream.TryRead(out byte[] d))
                return false;
            if (!stream.TryRead(out byte[] dp))
                return false;
            if (!stream.TryRead(out byte[] dq))
                return false;
            if (!stream.TryRead(out byte[] exponent))
                return false;
            if (!stream.TryRead(out byte[] inverseQ))
                return false;
            if (!stream.TryRead(out byte[] modulus))
                return false;
            if (!stream.TryRead(out byte[] p))
                return false;
            if (!stream.TryRead(out byte[] q))
                return false;
            key = new RSAParameters
            {
                D = d,
                DP = dp,
                DQ = dq,
                Exponent = exponent,
                InverseQ = inverseQ,
                Modulus = modulus,
                P = p,
                Q = q
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
            stream.Write(key.D);
            stream.Write(key.DP);
            stream.Write(key.DQ);
            stream.Write(key.Exponent);
            stream.Write(key.InverseQ);
            stream.Write(key.Modulus);
            stream.Write(key.P);
            stream.Write(key.Q);
        }
    }
}

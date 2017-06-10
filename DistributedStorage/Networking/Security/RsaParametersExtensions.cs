namespace DistributedStorage.Networking.Security
{
    using System;
    using System.Diagnostics;
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

        public static int GetKeySize(this RSAParameters key) => key.Modulus.Length;

        public static bool TryBlockingRead(this Stream stream, TimeSpan timeout, out RSAParameters key)
        {
            key = default(RSAParameters);
            var start = Stopwatch.StartNew();
            if (!stream.TryBlockingRead(timeout, out byte[] remoteExponent))
                return false;
            if (!stream.TryBlockingRead(timeout - start.Elapsed, out byte[] remoteModulus))
                return false;
            key = new RSAParameters
            {
                Exponent = remoteExponent,
                Modulus = remoteModulus
            };
            return true;
        }

        public static bool TryImmediateRead(this Stream stream, out RSAParameters key)
        {
            key = default(RSAParameters);
            if (!stream.TryImmediateRead(out byte[] remoteExponent))
                return false;
            if (!stream.TryImmediateRead(out byte[] remoteModulus))
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

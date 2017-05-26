namespace Security
{
    using System.IO;
    using System.Security.Cryptography;

    internal class SecureStream
    {
        public static SecureStream AcceptConnection(RSAParameters ours, Stream underlyingStream)
        {
            var theirs = Crypto.SwapPublicRsaKeys(underlyingStream, ours);
            // TODO: Determine whether we want to talk to them, based on their public key
        }
    }
}

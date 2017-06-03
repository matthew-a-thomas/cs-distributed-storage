namespace Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    internal static class RsaKeySwapperExtensions
    {
        /// <summary>
        /// Uses the given stream to write out the public part of our RSA key, and read in and returns the public part of their RSA key.
        /// This method also verifies that the sending party owns the private key for the public key they're sending.
        /// It does this by also swapping nonces, and signing/verifying them
        /// </summary>
        internal static bool TrySwapPublicRsaKeys(this RsaKeySwapper @this, Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs)
        {
            // Send our stuff
            @this.SendOurPublicKey(underlyingStream, ours);

            // Read and verify their stuff
            return @this.TryGetTheirPublicKey(underlyingStream, timeout, out theirs);
        }

    }
}

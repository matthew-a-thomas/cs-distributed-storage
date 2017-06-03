namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    internal static class RsaKeySwapper
    {
        /// <summary>
        /// Uses the given stream to write out the public part of our RSA key, and read in and returns the public part of their RSA key.
        /// This method also verifies that the sending party owns the private key for the public key they're sending.
        /// It does this by also swapping nonces, and signing/verifying them
        /// </summary>
        internal static bool TrySwapPublicRsaKeys(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs)
        {
            // Send our stuff
            {
                // Write out our public key
                underlyingStream.WritePublicKey(ours);

                // Create a nonce and write it out
                var nonce = Crypto.CreateNonce(ours.GetKeySize());
                underlyingStream.WriteChunk(nonce);

                // Sign the nonce that we sent, and send that signature
                using (var rsa = ours.CreateRsa())
                {
                    var signature = rsa.SignData(nonce, Crypto.HashName, Crypto.SignaturePadding);
                    underlyingStream.WriteChunk(signature);
                }
            }

            // Read and verify their stuff
            {
                // Read in what they sent
                var start = Stopwatch.StartNew();
                var remotePublicKey = underlyingStream.ReadPublicKey(timeout);
                var nonce = underlyingStream.BlockingReadChunk(timeout -= start.Elapsed); // Read the nonce they sent. We don't actually do anything with that

                // See if the nonce length is valid
                var isValid = nonce.Length == remotePublicKey.GetKeySize();

                // Now read out the signature they sent
                var signature = underlyingStream.BlockingReadChunk(timeout - start.Elapsed);

                // See if the signature of the nonce is good
                using (var rsa = remotePublicKey.CreateRsa())
                {
                    isValid &= rsa.VerifyData(nonce, signature, Crypto.HashName, Crypto.SignaturePadding);

                    // Go ahead and output the public key they sent
                    theirs = remotePublicKey;

                    // Finally, return whether everything is kosher
                    return isValid;
                }
            }
        }
    }
}

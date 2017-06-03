namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    internal class RsaKeySwapper
    {
        /// <summary>
        /// Performs RSA crypto stuff for us
        /// </summary>
        private readonly ICryptoRsa _cryptoRsa;

        /// <summary>
        /// Our source of entropy
        /// </summary>
        private readonly IEntropy _entropy;

        /// <summary>
        /// Creates a new <see cref="RsaKeySwapper"/> that uses the given RSA crypto and entropy source
        /// </summary>
        internal RsaKeySwapper(ICryptoRsa cryptoRsa, IEntropy entropy)
        {
            _cryptoRsa = cryptoRsa;
            _entropy = entropy;
        }

        /// <summary>
        /// Uses the given stream to write out the public part of our RSA key, and read in and returns the public part of their RSA key.
        /// This method also verifies that the sending party owns the private key for the public key they're sending.
        /// It does this by also swapping nonces, and signing/verifying them
        /// </summary>
        internal bool TrySwapPublicRsaKeys(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs)
        {
            // Send our stuff
            {
                // Write out our public key
                underlyingStream.WritePublicKey(ours);

                // Create a nonce and write it out
                var nonce = _entropy.CreateNonce(ours.GetKeySize());
                underlyingStream.WriteChunk(nonce);

                // Sign the nonce that we sent, and send that signature
                var signature = _cryptoRsa.Sign(nonce, ours);
                underlyingStream.WriteChunk(signature);
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
                isValid &= _cryptoRsa.Verify(nonce, signature, remotePublicKey);

                // Go ahead and output the public key they sent
                theirs = remotePublicKey;

                // Finally, return whether everything is kosher
                return isValid;
            }
        }
    }
}

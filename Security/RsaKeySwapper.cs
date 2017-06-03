namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    // TODO: Need to prevent replay attacks

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
        /// Tries to receive the other party's public key.
        /// We read in their public key, then a nonce, then a signature of that nonce.
        /// We verify that the signature of the nonce was made with the private portion of the reported public key.
        /// In this way we can know that the other party owns the private key of the reported public key.
        /// </summary>
        internal bool TryGetTheirPublicKey(Stream stream, TimeSpan timeout, out RSAParameters theirs)
        {
            // Read in what they sent
            var start = Stopwatch.StartNew();
            var remotePublicKey = stream.ReadPublicKey(timeout);
            var nonce = stream.BlockingReadChunk(timeout -= start.Elapsed); // Read the nonce they sent. We don't actually do anything with that

            // See if the nonce length is valid
            var isValid = nonce.Length == remotePublicKey.GetKeySize();

            // Now read out the signature they sent
            var signature = stream.BlockingReadChunk(timeout - start.Elapsed);

            // See if the signature of the nonce is good
            isValid &= _cryptoRsa.Verify(nonce, signature, remotePublicKey);

            // Go ahead and output the public key they sent
            theirs = remotePublicKey;

            // Finally, return whether everything is kosher
            return isValid;
        }

        /// <summary>
        /// Sends our public key followed by a nonce and our signature of that nonce.
        /// This will prove to others that we own the private key to the public key we sent
        /// </summary>
        internal void SendOurPublicKey(Stream stream, RSAParameters ours)
        {
            // Write out our public key
            stream.WritePublicKey(ours);

            // Create a nonce and write it out
            var nonce = _entropy.CreateNonce(ours.GetKeySize());
            stream.WriteChunk(nonce);

            // Sign the nonce that we sent, and send that signature
            var signature = _cryptoRsa.Sign(nonce, ours);
            stream.WriteChunk(signature);
        }
    }
}

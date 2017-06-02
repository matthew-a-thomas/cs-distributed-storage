namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    internal class SecureStream
    {
        #region Static methods
        
        /// <summary>
        /// Attempts to accept a connection over the given <paramref name="underlyingStream"/>.
        /// False is returned if we can't prove that the other party owns the private key for their public key
        /// </summary>
        public static bool TryAcceptConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;

            // Try swapping public keys and verifying that the other party owns the corresponding private key
            var start = Stopwatch.StartNew();
            if (!Crypto.TrySwapPublicRsaKeys(underlyingStream, ours, timeout, out theirs))
                return false;

            // Now that we have their public key, and know that they have the corresponding private key, let's wait for them to tell us what the connection key is
            var ciphertext = underlyingStream.BlockingReadChunk(timeout - start.Elapsed);
            var connectionKey = Crypto.DecryptRsa(ciphertext, ours, theirs); // Try decrypting the connection key that they should have sent
            if (connectionKey == null)
                return false; // We couldn't decrypt what they sent. Perhaps their signature is wrong, or perhaps something else is wrong

            // Now we're ready to create a SecureStream
            secureStream = new SecureStream(underlyingStream, connectionKey);
            return true;
        }

        /// <summary>
        /// Attempts to make a connection over the given <paramref name="underlyingStream"/>.
        /// False is returned if we can't prove that the other party owns the private key for their public key
        /// </summary>
        public static bool TryMakeConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;

            // Try swapping public keys and verifying that the other party owns the corresponding private key
            if (!Crypto.TrySwapPublicRsaKeys(underlyingStream, ours, timeout, out theirs))
                return false;

            // Now that we have their public key, and know that they have the corresponding private key, let's tell them what the connection key will be
            var connectionKey = Crypto.CreateAesKey(); // First, let's make one up
            var ciphertext = Crypto.EncryptRsa(connectionKey, ours, theirs); // Next let's encrypt and sign it
            underlyingStream.WriteChunk(ciphertext); // Send it along

            // Now we're ready to create a SecureStream
            secureStream = new SecureStream(underlyingStream, connectionKey);
            return true;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="SecureStream"/> through which messages can be securely sent over the given <paramref name="underlyingStream"/>
        /// </summary>
        public SecureStream(Stream underlyingStream, byte[] connectionKey)
        {
            _underlyingStream = underlyingStream;
            _connectionKey = connectionKey;
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Sends the given <paramref name="data"/> in encrypted form to the other party
        /// </summary>
        public void SendDatagram(byte[] data)
        {
            // Create an ephemeral session key
            var sessionKey = Crypto.CreateAesKey();
            // Encrypt+HMAC the session key using our connection key
            var ciphertextSessionKey = Crypto.EncryptAes(sessionKey, _connectionKey);
            // Encrypt+HMAC the given data using the session key
            var ciphertext = Crypto.EncryptAes(data, sessionKey);

            // Send the encrypted session key
            _underlyingStream.WriteChunk(ciphertextSessionKey);
            // Send the encrypted data
            _underlyingStream.WriteChunk(ciphertext);
        }

        /// <summary>
        /// Tries to receive an encrypted message from the other party
        /// </summary>
        public bool TryReceiveDatagram(TimeSpan timeout, out byte[] data)
        {
            data = null;

            // Read the encrypted session key and try to decrypt it using the connection key
            var start = Stopwatch.StartNew();
            var ciphertextSessionKey = _underlyingStream.BlockingReadChunk(timeout);
            var sessionKey = Crypto.DecryptAes(ciphertextSessionKey, _connectionKey);
            if (sessionKey == null)
                return false;

            // Try to decrypt the ciphertext using the session key
            var ciphertext = _underlyingStream.BlockingReadChunk(timeout - start.Elapsed);
            data = Crypto.DecryptAes(ciphertext, sessionKey);
            return data != null;
        }

        #endregion

        #region Private fields

        /// <summary>
        /// The AES encryption key to use for this connection
        /// </summary>
        private readonly byte[] _connectionKey;

        /// <summary>
        /// The <see cref="Stream"/> through which we will communicate
        /// </summary>
        private readonly Stream _underlyingStream;

        #endregion
    }
}

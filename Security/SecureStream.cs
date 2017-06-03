﻿namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Common;

    public class SecureStream
    {
        #region Constructor

        /// <summary>
        /// Creates a new <see cref="SecureStream"/> through which messages can be securely sent over the given <paramref name="underlyingStream"/>
        /// </summary>
        public SecureStream(Stream underlyingStream, byte[] connectionKey)
        {
            _underlyingStream = underlyingStream;
            _connectionKey = Crypto.ConvertToAesKey(connectionKey);
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

namespace DistributedStorage.Networking.Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Common;

    /// <summary>
    /// Something that uses symmetric encryption to send and receive data over a <see cref="Stream"/>
    /// </summary>
    public class SecureStream
    {
        #region Constructor

        /// <summary>
        /// Creates a new <see cref="SecureStream"/> through which messages can be securely sent over the given <paramref name="underlyingStream"/>
        /// </summary>
        public SecureStream(Stream underlyingStream, byte[] connectionKey, ICryptoSymmetric cryptoSymmetric)
        {
            _underlyingStream = underlyingStream;
            _cryptoSymmetric = cryptoSymmetric;
            _connectionKey = _cryptoSymmetric.ConvertToAesKey(connectionKey);
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Sends the given <paramref name="data"/> in encrypted form to the other party
        /// </summary>
        public void SendDatagram(byte[] data)
        {
            // Create an ephemeral session key
            var sessionKey = _cryptoSymmetric.CreateAesKey();
            // Encrypt+HMAC the session key using our connection key
            var ciphertextSessionKey = _cryptoSymmetric.EncryptAndHmac(sessionKey, _connectionKey);
            // Encrypt+HMAC the given data using the session key
            var ciphertext = _cryptoSymmetric.EncryptAndHmac(data, sessionKey);

            // Send the encrypted session key
            _underlyingStream.Write(ciphertextSessionKey);
            // Send the encrypted data
            _underlyingStream.Write(ciphertext);
        }

        /// <summary>
        /// Tries to receive an encrypted message from the other party
        /// </summary>
        public bool TryReceiveDatagram(TimeSpan timeout, out byte[] data)
        {
            data = null;
            
            // Read the encrypted session key and try to decrypt it using the connection key
            var start = Stopwatch.StartNew();
            if (!_underlyingStream.TryBlockingRead(timeout, out byte[] ciphertextSessionKey))
                return false;
            if (!_cryptoSymmetric.TryVerifyHmacAndDecrypt(ciphertextSessionKey, _connectionKey, out var sessionKey))
                return false;
            if (sessionKey == null)
                return false;

            // Try to decrypt the ciphertext using the session key
            return
                _underlyingStream.TryBlockingRead(timeout - start.Elapsed, out byte[] ciphertext)
                &&
                _cryptoSymmetric.TryVerifyHmacAndDecrypt(ciphertext, sessionKey, out data);
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Performs crypto functions for us
        /// </summary>
        private readonly ICryptoSymmetric _cryptoSymmetric;

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

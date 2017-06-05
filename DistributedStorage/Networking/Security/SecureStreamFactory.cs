namespace DistributedStorage.Networking.Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

    /// <summary>
    /// Creates <see cref="SecureStream"/>s for us with the help of a <see cref="RsaKeySwapper"/> and RSA keys
    /// </summary>
    public sealed class SecureStreamFactory
    {
        /// <summary>
        /// The creation mode for making a new <see cref="SecureStream"/> through the <see cref="SecureStreamFactory"/>
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Accept an incoming connection
            /// </summary>
            Accept,

            /// <summary>
            /// Make an outgoing connection
            /// </summary>
            Make
        }

        #region Private fields

        /// <summary>
        /// Performs AES crypto
        /// </summary>
        private readonly ICryptoSymmetric _cryptoSymmetric;

        /// <summary>
        /// Performs RSA crypto for us
        /// </summary>
        private readonly ICryptoRsa _cryptoRsa;

        /// <summary>
        /// Our source of entrupy
        /// </summary>
        private readonly IEntropy _entropy;

        /// <summary>
        /// Handles swapping RSA keys over a stream
        /// </summary>
        private readonly RsaKeySwapper _keySwapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="SecureStreamFactory"/>
        /// </summary>
        public SecureStreamFactory(ICryptoRsa cryptoRsa, ICryptoSymmetric cryptoSymmetric, IEntropy entropy)
        {
            _cryptoRsa = cryptoRsa;
            _cryptoSymmetric = cryptoSymmetric;
            _entropy = entropy;
            _keySwapper = new RsaKeySwapper(cryptoRsa);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The same as calling either <see cref="TryAcceptConnection"/> or <see cref="TryMakeConnection"/>, but with a <see cref="Mode"/> that determines whether a connection is being accepted or made
        /// </summary>
        public bool TryCreateConnection(
            Stream underlyingStream,
            RSAParameters ours,
            Mode mode,
            TimeSpan timeout,
            out RSAParameters theirs,
            out SecureStream secureStream)
        {
            switch (mode)
            {
                case Mode.Accept:
                    return TryAcceptConnection(underlyingStream, ours, timeout, out theirs, out secureStream);
                case Mode.Make:
                    return TryMakeConnection(underlyingStream, ours, timeout, out theirs, out secureStream);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Attempts to accept a connection over the given <paramref name="underlyingStream"/>.
        /// False is returned if we can't prove that the other party owns the private key for their public key
        /// </summary>
        public bool TryAcceptConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;
            
            // Try swapping public keys and verifying that the other party owns the corresponding private key
            var start = Stopwatch.StartNew();
            if (!_keySwapper.TrySwapPublicRsaKeys(underlyingStream, ours, timeout, _entropy, out theirs))
                return false;

            // Now that we have their public key, and know that they have the corresponding private key, let's wait for them to tell us what the connection key is
            if (!underlyingStream.TryBlockingRead(timeout - start.Elapsed, out byte[] ciphertext))
                return false;
            if (!_cryptoRsa.TryDecryptRsa(ciphertext, ours, theirs, out var connectionKey)) // Try decrypting the connection key that they should have sent
                return false;

            // Now we're ready to create a SecureStream
            secureStream = new SecureStream(underlyingStream, connectionKey, _cryptoSymmetric);
            return true;
        }

        /// <summary>
        /// Attempts to make a connection over the given <paramref name="underlyingStream"/>.
        /// False is returned if we can't prove that the other party owns the private key for their public key
        /// </summary>
        public bool TryMakeConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;
            
            // Try swapping public keys and verifying that the other party owns the corresponding private key
            if (!_keySwapper.TrySwapPublicRsaKeys(underlyingStream, ours, timeout, _entropy, out theirs))
                return false;

            // Now that we have their public key, and know that they have the corresponding private key, let's tell them what the connection key will be
            var connectionKey = _cryptoSymmetric.CreateAesKey(); // First, let's make one up
            var ciphertext = _cryptoRsa.EncryptRsa(connectionKey, ours, theirs); // Next let's encrypt and sign it
            underlyingStream.Write(ciphertext); // Send it along

            // Now we're ready to create a SecureStream
            secureStream = new SecureStream(underlyingStream, connectionKey, _cryptoSymmetric);
            return true;
        }

        #endregion
    }
}

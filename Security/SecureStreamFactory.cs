namespace Security
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using Common;

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

        #region Methods

        /// <summary>
        /// The same as calling either <see cref="TryAcceptConnection"/> or <see cref="TryMakeConnection"/>, but with a <see cref="Mode"/> that determines whether a connection is being accepted or made
        /// </summary>
        public static bool TryCreateConnection(
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
        public static bool TryAcceptConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;

            try
            {
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
            catch
            {
                theirs = default(RSAParameters);
                return false;
            }
        }

        /// <summary>
        /// Attempts to make a connection over the given <paramref name="underlyingStream"/>.
        /// False is returned if we can't prove that the other party owns the private key for their public key
        /// </summary>
        public static bool TryMakeConnection(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs, out SecureStream secureStream)
        {
            // Start out assuming that we'll fail
            secureStream = null;

            try
            {
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
            catch
            {
                theirs = default(RSAParameters);
                return false;
            }
        }

        #endregion
    }
}

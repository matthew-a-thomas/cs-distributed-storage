namespace Security
{
    using System;
    using System.Diagnostics;
    using Common;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// Contains many cryptographic functions that can be used to build a protocol
    /// </summary>
    internal static class Crypto
    {
        internal static readonly RSAEncryptionPadding EncryptionPadding = RSAEncryptionPadding.Pkcs1;

        /// <summary>
        /// The name of the hash algorithm we'll be working with
        /// </summary>
        internal static readonly HashAlgorithmName HashName = HashAlgorithmName.SHA512;

        internal static readonly RSASignaturePadding SignaturePadding = RSASignaturePadding.Pkcs1;

        /// <summary>
        /// Creates an <see cref="Aes"/> and sets various parameters
        /// </summary>
        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        /// <summary>
        /// Creates a nonce that can be used as the IV of an <see cref="Aes"/>
        /// </summary>
        private static byte[] CreateAesIv() => CreateNonce(128 / 8);

        /// <summary>
        /// Creates a nonce that can be used as the key of an <see cref="Aes"/>
        /// </summary>
        internal static byte[] CreateAesKey() => CreateNonce(256 / 8);

        /// <summary>
        /// Creates a new <see cref="HashAlgorithm"/>
        /// </summary>
        internal static HashAlgorithm CreateHashAlgorithm() => SHA512.Create();

        /// <summary>
        /// Creates a new <see cref="HMAC"/>
        /// </summary>
        private static HMAC CreateHmac() => new HMACSHA512();

        /// <summary>
        /// Creates a new <see cref="HMAC"/> with the given <paramref name="key"/>
        /// </summary>
        private static HMAC CreateHmac(byte[] key)
        {
            var hmac = CreateHmac();
            hmac.Key = key;
            return hmac;
        }

        /// <summary>
        /// Creates a cryptographically-secure random array of bytes
        /// </summary>
        private static byte[] CreateNonce(int size)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var nonce = new byte[size];
                rng.GetBytes(nonce);
                return nonce;
            }
        }

        /// <summary>
        /// Creates a new RSA private (and public) key
        /// </summary>
        internal static RSAParameters CreateRsaKey()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = 4096;
                return rsa.ExportParameters(true);
            }
        }

        /// <summary>
        /// Returns the given <paramref name="data"/> after being AES-decrypted (and HMAC verified) with the given <paramref name="key"/>
        /// </summary>
        internal static byte[] DecryptAes(byte[] data, byte[] key)
        {
            // Set up AES
            using (var aes = CreateAes())
            {
                using (var buffer = new MemoryStream(data))
                {
                    // Pull out the initialization vector and ciphertext
                    var iv = buffer.BlockingReadChunk(TimeSpan.MaxValue);
                    var ciphertext = buffer.BlockingReadChunk(TimeSpan.MaxValue);

                    // Assert that the HMAC is correct
                    using (var hasher = CreateHmac(key))
                    {
                        var lengthOfFirstPart = (int)buffer.Position;
                        var reportedHmac = buffer.BlockingReadChunk(TimeSpan.MaxValue);
                        var computedHmac = hasher.ComputeHash(data, 0, lengthOfFirstPart);
                        if (!computedHmac.SequenceEqual(reportedHmac))
                            return null;
                    }

                    // Set up an AES decryptor
                    using (var decryptor = aes.CreateDecryptor(key, iv))
                    {
                        // Decrypt the ciphertext
                        var plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

                        // Return the result
                        return plaintext;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the given <paramref name="data"/> after being RSA decrypted with our private key and after the signature has been verified with their public key
        /// </summary>
        internal static byte[] DecryptRsa(byte[] data, RSAParameters ours, RSAParameters theirs)
        {
            using (var ourRsa = ours.CreateRsa())
            {
                using (var theirRsa = theirs.CreateRsa())
                {
                    using (var buffer = new MemoryStream(data))
                    {
                        // Pull out the ciphertext
                        var ciphertext = buffer.BlockingReadChunk(TimeSpan.MaxValue);

                        // Verify the signature
                        var lengthOfFirstPart = (int)buffer.Position;
                        var signature = buffer.BlockingReadChunk(TimeSpan.MaxValue);
                        if (!theirRsa.VerifyData(data, 0, lengthOfFirstPart, signature, HashName, SignaturePadding))
                            return null;

                        // Decrypt the ciphertext
                        var plaintext = ourRsa.Decrypt(ciphertext, EncryptionPadding);

                        // Return the result
                        return plaintext;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the given <paramref name="data"/> after being AES-encrypted and HMAC'ed with the given <paramref name="key"/>
        /// </summary>
        internal static byte[] EncryptAes(byte[] data, byte[] key)
        {
            // Set up AES
            using (var aes = CreateAes())
            {
                // Create an initialization vector
                var iv = CreateAesIv();

                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    using (var buffer = new MemoryStream())
                    {
                        // First, write out the IV
                        buffer.WriteChunk(iv);

                        // Next, write out the AES-encrypted ciphertext
                        buffer.WriteChunk(encryptor.TransformFinalBlock(data, 0, data.Length));

                        // Reset the MemoryStream to prepare for HMAC'ing
                        buffer.Position = 0;

                        using (var hasher = CreateHmac(key))
                        {
                            // Write out the HMAC of what we've written so far
                            var hmac = hasher.ComputeHash(buffer);
                            buffer.WriteChunk(hmac);

                            // Return the result
                            return buffer.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the given <paramref name="data"/> after being RSA encrypted with their public key and signed with our private key
        /// </summary>
        internal static byte[] EncryptRsa(byte[] data, RSAParameters ours, RSAParameters theirs)
        {
            // Set up an RSA with our private key
            using (var ourRsa = ours.CreateRsa())
            {
                // Set up an RSA with their public key
                using (var theirRsa = theirs.CreateRsa())
                {
                    using (var buffer = new MemoryStream())
                    {
                        // Encrypt the given data using their public key, then write out the ciphertext length and the ciphertext contents into the buffer memory stream
                        buffer.WriteChunk(theirRsa.Encrypt(data, EncryptionPadding));

                        // Reset the stream's position in preparation of hashing/signing
                        buffer.Position = 0;

                        // Sign what we've written
                        var signature = ourRsa.SignData(buffer, HashName, SignaturePadding);

                        // Now write out our signature
                        buffer.WriteChunk(signature);

                        // Return the result
                        return buffer.ToArray();
                    }
                }
            }
        }
        
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
                var nonce = CreateNonce(ours.GetKeySize());
                underlyingStream.WriteChunk(nonce);

                // Sign the nonce that we sent, and send that signature
                using (var rsa = ours.CreateRsa())
                {
                    var signature = rsa.SignData(nonce, HashName, SignaturePadding);
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
                    isValid &= rsa.VerifyData(nonce, signature, HashName, SignaturePadding);

                    // Go ahead and output the public key they sent
                    theirs = remotePublicKey;

                    // Finally, return whether everything is kosher
                    return isValid;
                }
            }
        }
    }
}

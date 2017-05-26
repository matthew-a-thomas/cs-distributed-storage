namespace Security
{
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    internal static class Crypto
    {
        /// <summary>
        /// Creates an <see cref="Aes"/> and sets various parameters
        /// </summary>
        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        /// <summary>
        /// Creates a nonce that can be used as either the IV or key of an <see cref="Aes"/>
        /// </summary>
        private static byte[] CreateAesIvOrKey() => CreateNonce(256 / 8);

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
        internal static byte[] CreateNonce(int size)
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
        internal static byte[] Decrypt(byte[] data, byte[] key)
        {
            // Set up AES
            using (var aes = CreateAes())
            {
                using (var buffer = new MemoryStream(data))
                {
                    // Pull out the initialization vector and ciphertext
                    var iv = buffer.ReadChunk();
                    var ciphertext = buffer.ReadChunk();

                    // Assert that the HMAC is correct
                    using (var hasher = CreateHmac())
                    {
                        var lengthOfFirstPart = (int)buffer.Position;
                        var reportedHmac = buffer.ReadChunk();
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
        internal static byte[] Decrypt(byte[] data, RSAParameters ours, RSAParameters theirs)
        {
            using (var ourRsa = RSA.Create())
            {
                ourRsa.ImportParameters(ours);

                using (var theirRsa = RSA.Create())
                {
                    theirRsa.ImportParameters(theirs);

                    using (var buffer = new MemoryStream(data))
                    {
                        // Pull out the ciphertext
                        var ciphertext = buffer.ReadChunk();

                        // Verify the signature
                        var lengthOfFirstPart = (int)buffer.Position;
                        var signature = buffer.ReadChunk();
                        if (!theirRsa.VerifyData(data, 0, lengthOfFirstPart, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1))
                            return null;

                        // Decrypt the ciphertext
                        var plaintext = ourRsa.Decrypt(ciphertext, RSAEncryptionPadding.Pkcs1);

                        // Return the result
                        return plaintext;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the given <paramref name="data"/> after being AES-encrypted and HMAC'ed with the given <paramref name="key"/>
        /// </summary>
        internal static byte[] Encrypt(byte[] data, byte[] key)
        {
            // Set up AES
            using (var aes = CreateAes())
            {
                // Create an initialization vector
                var iv = CreateAesIvOrKey();

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
                            buffer.WriteChunk(hasher.ComputeHash(buffer));

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
        internal static byte[] Encrypt(byte[] data, RSAParameters ours, RSAParameters theirs)
        {
            // Set up an RSA with our private key
            using (var ourRsa = RSA.Create())
            {
                ourRsa.ImportParameters(ours);

                // Set up an RSA with their public key
                using (var theirRsa = RSA.Create())
                {
                    theirRsa.ImportParameters(theirs);

                    using (var buffer = new MemoryStream())
                    {
                        // Encrypt the given data using their public key, then write out the ciphertext length and the ciphertext contents into the buffer memory stream
                        buffer.WriteChunk(theirRsa.Encrypt(data, RSAEncryptionPadding.Pkcs1));

                        // Reset the stream's position in preparation of hashing/signing
                        buffer.Position = 0;

                        // Sign what we've written
                        var signature = ourRsa.SignData(buffer, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

                        // Now write out our signature
                        buffer.WriteChunk(signature);

                        // Return the result
                        return buffer.ToArray();
                    }
                }
            }
        }
        
        /// <summary>
        /// Uses the given stream to write out the public part of our RSA key, and read in and returns the public part of their RSA key
        /// </summary>
        internal static RSAParameters SwapPublicRsaKeys(Stream underlyingStream, RSAParameters ours)
        {
            // TODO: Swap a signed nonce

            // Write out our public key
            underlyingStream.Write(ours.ToBytes());

            // Read in their public key
            var remotePublicKey = underlyingStream.ReadPublicKey();

            // Return their public key
            return remotePublicKey;
        }
    }
}

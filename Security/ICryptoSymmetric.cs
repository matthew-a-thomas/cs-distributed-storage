namespace Security
{
    /// <summary>
    /// Performs symmetric cryptographic functions
    /// </summary>
    public interface ICryptoSymmetric
    {
        /// <summary>
        /// Creates a new key that is suitable for use within this <see cref="ICryptoSymmetric"/>
        /// </summary>
        byte[] CreateAesKey();

        /// <summary>
        /// Converts the given byte array into a key that is suitable for use within this <see cref="ICryptoSymmetric"/>.
        /// The same input should always produce the same output.
        /// </summary>
        byte[] ConvertToAesKey(byte[] key);

        /// <summary>
        /// Encrypts and HMACs the given <paramref name="plaintext"/> using the given <paramref name="key"/>
        /// </summary>
        byte[] EncryptAndHmac(byte[] plaintext, byte[] key);

        /// <summary>
        /// Verifies the HMAC and decrypts the given <paramref name="ciphertext"/> using the given <paramref name="key"/>
        /// </summary>
        bool TryVerifyHmacAndDecrypt(byte[] ciphertext, byte[] key, out byte[] plaintext);
    }
}

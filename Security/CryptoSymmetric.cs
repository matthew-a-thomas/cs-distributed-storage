namespace Security
{
    /// <summary>
    /// An implementation of <see cref="ICryptoSymmetric"/> that uses AES-256 behind the scenes
    /// </summary>
    public class CryptoSymmetric : ICryptoSymmetric
    {
        /// <summary>
        /// Returns a cryptographically-secure random array of 256/8 bytes
        /// </summary>
        public byte[] CreateAesKey() => Crypto.CreateAesKey();

        /// <summary>
        /// Performs SHA-256 on the given <paramref name="key"/> to turn it into a key that can be used for AES-256
        /// </summary>
        public byte[] ConvertToAesKey(byte[] key) => Crypto.ConvertToAesKey(key);

        /// <summary>
        /// Performs AES encryption and HMAC on the given <paramref name="plaintext"/> using the given <paramref name="key"/>, an AES-256 cipher, and a SHA512 HMAC
        /// </summary>
        public byte[] EncryptAndHmac(byte[] plaintext, byte[] key) => Crypto.EncryptAes(plaintext, key);

        /// <summary>
        /// Performs AES decryption and HMAC validation of the given <paramref name="ciphertext"/> using the given <paramref name="key"/>, an AES-256 algorithm, and a SHA512 HMAC
        /// </summary>
        public bool TryVerifyHmacAndDecrypt(byte[] ciphertext, byte[] key, out byte[] plaintext) => Crypto.TryDecryptAes(ciphertext, key, out plaintext);
    }
}

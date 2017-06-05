namespace DistributedStorage.Security
{
    using System.Security.Cryptography;

    /// <summary>
    /// Performs RSA cryptographic functions
    /// </summary>
    public interface ICryptoRsa
    {
        /// <summary>
        /// Creates a new RSA key that can be used in this <see cref="ICryptoRsa"/> instance
        /// </summary>
        /// <returns></returns>
        RSAParameters CreateKey();

        /// <summary>
        /// Performs RSA encryption on the given <paramref name="plaintext"/>.
        /// Implementations should encrypt it using the public key of <paramref name="theirs"/> and sign it using the private key of <paramref name="ours"/>
        /// </summary>
        byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs);

        /// <summary>
        /// Signs the given <paramref name="data"/> using the private key of <paramref name="ours"/>
        /// </summary>
        byte[] Sign(byte[] data, RSAParameters ours);

        /// <summary>
        /// Performs RSA decryption on the given <paramref name="ciphertext"/>.
        /// Implementations should guarantee that it was encrypted using the public key of <paramref name="ours"/> and signed using the private key of <paramref name="theirs"/>
        /// </summary>
        bool TryDecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs, out byte[] plaintext);

        /// <summary>
        /// Verifies that the given <paramref name="data"/> was signed by the private key of <paramref name="theirs"/> to produce the given <paramref name="signature"/>
        /// </summary>
        bool Verify(byte[] data, byte[] signature, RSAParameters theirs);
    }
}

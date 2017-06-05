namespace Security
{
    using System.Security.Cryptography;

    /// <summary>
    /// An implementation of <see cref="ICryptoRsa"/>
    /// </summary>
    public sealed class CryptoRsa : ICryptoRsa
    {
        public RSAParameters CreateKey() => Crypto.CreateRsaKey();

        /// <summary>
        /// Encrypts the given <paramref name="plaintext"/> using the public key of <paramref name="theirs"/> and signs it with <paramref name="ours"/>
        /// </summary>
        public byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs) => Crypto.EncryptRsa(plaintext, ours, theirs);

        /// <summary>
        /// Signs the given <paramref name="data"/> with the private key of <paramref name="ours"/> and a SHA512 hash algorithm
        /// </summary>
        public byte[] Sign(byte[] data, RSAParameters ours)
        {
            using (var rsa = ours.CreateRsa())
                return rsa.SignData(data, Crypto.HashName, Crypto.SignaturePadding);
        }

        /// <summary>
        /// Decrypts the given <paramref name="ciphertext"/> using the private key of <paramref name="ours"/>, and verifies that it was signed with <paramref name="theirs"/>
        /// </summary>
        public bool TryDecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs, out byte[] plaintext) => Crypto.TryDecryptRsa(ciphertext, ours, theirs, out plaintext);

        /// <summary>
        /// Validates the given <paramref name="signature"/> came from the given <paramref name="data"/> using <paramref name="theirs"/> and a SHA512 hash algorithm
        /// </summary>
        public bool Verify(byte[] data, byte[] signature, RSAParameters theirs)
        {
            using (var rsa = theirs.CreateRsa())
                return rsa.VerifyData(data, signature, Crypto.HashName, Crypto.SignaturePadding);
        }
    }
}

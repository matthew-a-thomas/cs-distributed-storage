namespace SecurityTests.Utils
{
    using System.Security.Cryptography;
    using Security;

    internal class NonsecureCryptoRsa : ICryptoRsa
    {
        /// <summary>
        /// Just returns the <paramref name="ciphertext"/>
        /// </summary>
        public bool TryDecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs, out byte[] plaintext)
        {
            plaintext = ciphertext;
            return true;
        }

        /// <summary>
        /// Just returns the <paramref name="plaintext"/>
        /// </summary>
        public byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs) => plaintext;

        /// <summary>
        /// Returns an empty byte array
        /// </summary>
        public byte[] Sign(byte[] data, RSAParameters ours) => new byte[0];

        /// <summary>
        /// Returns true
        /// </summary>
        public bool Verify(byte[] data, byte[] signature, RSAParameters theirs) => true;
    }
}

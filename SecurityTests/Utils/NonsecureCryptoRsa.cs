namespace SecurityTests.Utils
{
    using System.Security.Cryptography;
    using Security;

    internal class NonsecureCryptoRsa : ICryptoRsa
    {
        /// <summary>
        /// Just returns the <paramref name="ciphertext"/>
        /// </summary>
        public byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs) => ciphertext;

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

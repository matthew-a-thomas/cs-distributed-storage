namespace Security
{
    internal class NonsecureCryptoAes : ICryptoAes
    {
        /// <summary>
        /// Returns an array of 256/8 bytes filled with zeros
        /// </summary>
        public byte[] CreateAesKey() => new byte[256 / 8];

        /// <summary>
        /// The same as calling <see cref="CreateAesKey"/>
        /// </summary>
        public byte[] ConvertToAesKey(byte[] key) => CreateAesKey();

        /// <summary>
        /// Just returns the <paramref name="plaintext"/>
        /// </summary>
        public byte[] EncryptAes(byte[] plaintext, byte[] key) => plaintext;

        /// <summary>
        /// Just returns the <paramref name="ciphertext"/>
        /// </summary>
        public byte[] DecryptAes(byte[] ciphertext, byte[] key) => ciphertext;
    }
}

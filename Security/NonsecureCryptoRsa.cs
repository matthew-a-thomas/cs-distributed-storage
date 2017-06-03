namespace Security
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    internal class NonsecureCryptoRsa : ICryptoRsa
    {
        private readonly RSAParameters _ours;
        private readonly RSAParameters _theirs;

        public NonsecureCryptoRsa(RSAParameters ours, RSAParameters theirs)
        {
            _ours = ours;
            _theirs = theirs;
        }

        /// <summary>
        /// Always returns true, and sets <paramref name="theirs"/> to the other <see cref="RSAParameters"/> that was given to the constructor
        /// </summary>
        public bool TrySwapPublicRsaKeys(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs)
        {
            theirs = ours.Modulus.SequenceEqual(_ours.Modulus) ? _theirs : _ours;
            return true;
        }

        /// <summary>
        /// Just returns the <paramref name="ciphertext"/>
        /// </summary>
        public byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs) => ciphertext;

        /// <summary>
        /// Just returns the <paramref name="plaintext"/>
        /// </summary>
        public byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs) => plaintext;
    }
}

namespace Security
{
    using System.Security.Cryptography;

    public class CryptoRsa : ICryptoRsa
    {
        public byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs) => Crypto.DecryptRsa(ciphertext, ours, theirs);

        public byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs) => Crypto.EncryptRsa(plaintext, ours, theirs);
    }
}

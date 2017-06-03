namespace Security
{
    using System.Security.Cryptography;

    public class CryptoRsa : ICryptoRsa
    {
        public byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs) => Crypto.DecryptRsa(ciphertext, ours, theirs);

        public byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs) => Crypto.EncryptRsa(plaintext, ours, theirs);

        public byte[] Sign(byte[] data, RSAParameters ours)
        {
            using (var rsa = ours.CreateRsa())
                return rsa.SignData(data, Crypto.HashName, Crypto.SignaturePadding);
        }

        public bool Verify(byte[] data, byte[] signature, RSAParameters theirs)
        {
            using (var rsa = theirs.CreateRsa())
                return rsa.VerifyData(data, signature, Crypto.HashName, Crypto.SignaturePadding);
        }
    }
}

namespace Security
{
    using System.Security.Cryptography;

    public interface ICryptoRsa
    {
        byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs);
        byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs);
    }
}

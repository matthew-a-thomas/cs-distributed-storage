namespace Security
{
    using System.Security.Cryptography;

    public interface ICryptoRsa
    {
        byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs);
        byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs);
        byte[] Sign(byte[] data, RSAParameters ours);
        bool Verify(byte[] data, byte[] signature, RSAParameters theirs);
    }
}

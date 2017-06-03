namespace Security
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    public interface ICryptoRsa
    {
        bool TrySwapPublicRsaKeys(Stream underlyingStream, RSAParameters ours, TimeSpan timeout, out RSAParameters theirs);
        byte[] DecryptRsa(byte[] ciphertext, RSAParameters ours, RSAParameters theirs);
        byte[] EncryptRsa(byte[] plaintext, RSAParameters ours, RSAParameters theirs);
    }
}

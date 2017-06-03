namespace Security
{
    public interface ICryptoAes
    {
        byte[] CreateAesKey();
        byte[] ConvertToAesKey(byte[] connectionKey);
        byte[] EncryptAes(byte[] plaintext, byte[] key);
        byte[] DecryptAes(byte[] ciphertext, byte[] key);
    }
}

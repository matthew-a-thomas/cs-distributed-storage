namespace Security
{
    public class CryptoAes : ICryptoAes
    {
        public byte[] CreateAesKey() => Crypto.CreateAesKey();

        public byte[] ConvertToAesKey(byte[] key) => Crypto.ConvertToAesKey(key);

        public byte[] EncryptAes(byte[] plaintext, byte[] key) => Crypto.EncryptAes(plaintext, key);

        public byte[] DecryptAes(byte[] ciphertext, byte[] key) => Crypto.DecryptAes(ciphertext, key);
    }
}

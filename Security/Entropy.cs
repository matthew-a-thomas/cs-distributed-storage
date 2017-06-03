namespace Security
{
    public class Entropy : IEntropy
    {
        public byte[] CreateNonce(int size) => Crypto.CreateNonce(size);
    }
}

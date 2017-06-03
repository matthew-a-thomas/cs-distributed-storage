namespace Security
{
    public interface IEntropy
    {
        byte[] CreateNonce(int size);
    }
}

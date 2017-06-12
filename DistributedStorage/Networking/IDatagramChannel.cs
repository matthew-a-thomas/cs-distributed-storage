namespace DistributedStorage.Networking
{
    public interface IDatagramChannel
    {
        void SendDatagram(byte[] data);
        bool TryReceiveDatagram(out byte[] data);
    }
}

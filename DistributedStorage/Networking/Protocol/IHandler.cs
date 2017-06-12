namespace DistributedStorage.Networking.Protocol
{
    public interface IHandler
    {
        byte[] Handle(byte[] parameter);
    }
}

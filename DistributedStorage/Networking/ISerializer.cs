namespace DistributedStorage.Networking
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T thing);
        bool TryDeserialize(byte[] bytes, out T thing);
    }
}

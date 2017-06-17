namespace DistributedStorage.Networking.Serialization
{
    public sealed class NothingSerializer : ISerializer<Nothing>
    {
        public byte[] Serialize(Nothing thing) => new byte[0];

        public bool TryDeserialize(byte[] bytes, out Nothing thing)
        {
            thing = null;
            return true;
        }
    }
}

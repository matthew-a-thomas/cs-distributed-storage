namespace DistributedStorage.Networking.Serialization
{
    public sealed class NothingSerializer : ISerializer<NothingSerializer>
    {
        public byte[] Serialize(NothingSerializer thing) => new byte[0];

        public bool TryDeserialize(byte[] bytes, out NothingSerializer thing)
        {
            thing = new NothingSerializer();
            return true;
        }
    }
}

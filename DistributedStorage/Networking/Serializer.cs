namespace DistributedStorage.Networking
{
    public sealed class Serializer<T> : ISerializer<T>
    {
        public delegate byte[] SerializeDelegate(T thing);
        public delegate bool TryDeserializeDelegate(byte[] bytes, out T thing);

        public sealed class Options
        {
            public SerializeDelegate Serialize { get; }
            public TryDeserializeDelegate TryDeserialize { get; }

            public Options(SerializeDelegate serialize, TryDeserializeDelegate tryDeserialize)
            {
                Serialize = serialize;
                TryDeserialize = tryDeserialize;
            }
        }

        private readonly Options _options;

        public Serializer(Options options)
        {
            _options = options;
        }

        public byte[] Serialize(T thing) => _options.Serialize(thing);

        public bool TryDeserialize(byte[] bytes, out T thing) => _options.TryDeserialize(bytes, out thing);
    }
}

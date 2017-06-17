namespace DistributedStorage.Networking.Serialization
{
    using Common;
    using System.IO;

    public sealed class ArraySerializer<T> : ISerializer<T[]>
    {
        private readonly ISerializer<T> _elementSerializer;

        public ArraySerializer(ISerializer<T> elementSerializer)
        {
            _elementSerializer = elementSerializer;
        }

        public byte[] Serialize(T[] things)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(things.Length);
                foreach (var thing in things)
                {
                    var thingBytes = _elementSerializer.Serialize(thing);
                    stream.Write(thingBytes);
                }
                return stream.ToArray();
            }
        }

        public bool TryDeserialize(byte[] bytes, out T[] things)
        {
            things = null;
            using (var stream = new MemoryStream(bytes))
            {
                if (!stream.TryRead(out int numThings))
                    return false;
                things = new T[numThings];
                for (var i = 0; i < numThings; ++i)
                {
                    if (!stream.TryRead(out byte[] thingBytes))
                        return false;
                    if (!_elementSerializer.TryDeserialize(thingBytes, out var thing))
                        return false;
                    things[i] = thing;
                }
                return true;
            }
        }
    }
}

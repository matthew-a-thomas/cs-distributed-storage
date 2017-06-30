namespace DistributedStorage.Common
{
    using System.Collections.Generic;

    public sealed class CollectionOfByteArrays
    {
        public IReadOnlyList<byte[]> ByteArrays { get; }

        public CollectionOfByteArrays(IReadOnlyList<byte[]> byteArrays)
        {
            ByteArrays = byteArrays;
        }
    }
}

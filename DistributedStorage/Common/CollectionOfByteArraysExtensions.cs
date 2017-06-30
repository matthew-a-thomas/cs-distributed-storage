namespace DistributedStorage.Common
{
    using System.IO;

    public static class CollectionOfByteArraysExtensions
    {
        public static bool TryRead(this Stream stream, out CollectionOfByteArrays methodCall)
        {
            methodCall = null;
            if (!stream.TryRead(out int numParameters))
                return false;
            var serializedParameterValues = new byte[numParameters][];
            methodCall = new CollectionOfByteArrays(serializedParameterValues);
            for (var i = 0; i < numParameters; ++i)
            {
                if (!stream.TryRead(out serializedParameterValues[i]))
                    return false;
            }
            return true;
        }

        public static void Write(this Stream stream, CollectionOfByteArrays methodCall)
        {
            stream.Write(methodCall.ByteArrays.Count);
            foreach (var serializedParameterValue in methodCall.ByteArrays)
                stream.Write(serializedParameterValue);
        }
    }
}

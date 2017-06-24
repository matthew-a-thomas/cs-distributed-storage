namespace DistributedStorage.Networking
{
    using Common;
    using System.IO;

    /// <summary>
    /// Adapts a <see cref="Stream"/> to be an <see cref="IDatagramChannel"/>
    /// </summary>
    public sealed class StreamToDatagramChannelAdapter : IDatagramChannel
    {
        /// <summary>
        /// The adapted <see cref="Stream"/>
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new <see cref="StreamToDatagramChannelAdapter"/> which adapts a <see cref="Stream"/> to be an <see cref="IDatagramChannel"/>
        /// </summary>
        public StreamToDatagramChannelAdapter(Stream stream)
        {
            _stream = stream;
        }

        public void SendDatagram(byte[] data) => _stream.Write(data);

        public bool TryReceiveDatagram(out byte[] data) => _stream.TryRead(out data);
    }
}

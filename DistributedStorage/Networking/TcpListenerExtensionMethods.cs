namespace DistributedStorage.Networking
{
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Common;

    public static class TcpListenerExtensionMethods
    {
        /// <summary>
        /// Asynchronously accepts a <see cref="TcpClient"/> from this <see cref="TcpListener"/>.
        /// When a client connects, an <see cref="IDatagramChannel"/> is returned, but wrapped in an <see cref="DisposableWrapper{T}"/>.
        /// This means the returned value should be disposed at some point so that the client can be disposed of
        /// </summary>
        public static async Task<DisposableWrapper<IDatagramChannel>> AcceptDatagramChannelAsync(this TcpListener listener)
        {
            var client = await listener.AcceptTcpClientAsync();
            var stream = client.GetStream();
            var channel = new StreamToDatagramChannelAdapter(stream);
            return new DisposableWrapper<IDatagramChannel>(channel, () =>
            {
                stream.Dispose();
                client.Dispose();
            });
        }
    }
}

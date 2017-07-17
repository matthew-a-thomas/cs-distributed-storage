namespace Client
{
    using System.Net;

    public sealed class Server
    {
        public Server(IPEndPoint endpoint)
        {
            Endpoint = endpoint;
        }

        public IPEndPoint Endpoint { get; }

        public override string ToString() => Endpoint.ToString();
    }
}

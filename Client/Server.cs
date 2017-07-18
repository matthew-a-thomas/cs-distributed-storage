namespace Client
{
    using System.Net;
    using DistributedStorage.Authentication;

    public sealed class Server
    {
        public IPEndPoint Endpoint { get; }

        public Credential OwnerCredential { get; }

        public Server(IPEndPoint endpoint, Credential ownerCredential)
        {
            Endpoint = endpoint;
            OwnerCredential = ownerCredential;
        }

        public override string ToString() => Endpoint.ToString();
    }
}

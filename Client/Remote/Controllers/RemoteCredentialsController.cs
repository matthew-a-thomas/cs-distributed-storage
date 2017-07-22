namespace Client.Remote.Controllers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Controllers;
    using Networking.Http;

    public sealed class RemoteCredentialsController : ICredentialController
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendRequestAsyncDelegate;

        public RemoteCredentialsController(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendRequestAsyncDelegate)
        {
            _sendRequestAsyncDelegate = sendRequestAsyncDelegate;
        }

        public async Task<Credential> GenerateCredentialAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/credential");
            var response = await _sendRequestAsyncDelegate(request);
            var credential = await response.GetContentsAsync<Credential>();
            return credential;
        }
    }
}

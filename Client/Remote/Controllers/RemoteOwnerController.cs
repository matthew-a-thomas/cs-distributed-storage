namespace Client.Remote.Controllers
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using DistributedStorage.Networking.Controllers;
    using Networking.Http;
    using Newtonsoft.Json;

    public sealed class RemoteOwnerController : IOwnerController
    {
        private const string BaseUrl = "/api/owner";

        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendRequestAsync;

        public RemoteOwnerController(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendRequestAsync)
        {
            _sendRequestAsync = sendRequestAsync;
        }

        public async Task<string> GetOwnerAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
            var response = await _sendRequestAsync(request);
            var ownerIdentity = await response.GetContentsAsync<string>();
            return ownerIdentity;
        }

        public async Task<bool> PutOwnerAsync(string owner)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, BaseUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(owner), Encoding.UTF8, "application/json")
            };
            var response = await _sendRequestAsync(request);
            var wasSuccessful = await response.GetContentsAsync<bool>();
            return wasSuccessful;
        }
    }
}

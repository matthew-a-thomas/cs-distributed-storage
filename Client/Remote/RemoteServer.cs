namespace Client.Remote
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Controllers;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Controllers;
    using Microsoft.Net.Http.Headers;
    using Networking.Http;

    public sealed class RemoteServer : IRemoteServer
    {
        public sealed class Factory
        {
            private readonly HttpRequestMessageAuthorizer _httpRequestMessageAuthorizer;
            private readonly Credential _credential;

            public Factory(HttpRequestMessageAuthorizer httpRequestMessageAuthorizer, Credential credential)
            {
                _httpRequestMessageAuthorizer = httpRequestMessageAuthorizer;
                _credential = credential;
            }

            public RemoteServer Create(IPEndPoint endpoint) => new RemoteServer(endpoint, _credential, _httpRequestMessageAuthorizer);
        }

        #region Public properties

        #endregion

        #region Private fields

        private readonly HttpClient _client;
        private readonly Credential _credential;
        private readonly HttpRequestMessageAuthorizer _httpRequestMessageAuthorizer;

        #endregion

        #region Constructor

        public RemoteServer(IPEndPoint endpoint, Credential credential, HttpRequestMessageAuthorizer httpRequestMessageAuthorizer)
        {
            // Set up the client based on the given endpoint
            _client = new HttpClient {
                BaseAddress = new Uri($"http://{endpoint}")
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Matt's distributed storage client");

            // Store credentials
            _credential = credential;
            _httpRequestMessageAuthorizer = httpRequestMessageAuthorizer;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Takes care of adding authorization to the given <paramref name="request"/> before sending it off to the <see cref="_client"/>
        /// </summary>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            // Add authorization to this request if we have a credential
            Credential credential; // Prevent race condition of _credential maybe changing to null after we perform a null check (in case I want to make _credential non-readonly later)
            if ((credential = _credential) != null)
            {
                // Generate a random nonce
                var nonce = new byte[8];
                using (var rng = RandomNumberGenerator.Create())
                    rng.GetBytes(nonce);

                // Grab the current time
                var unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

                // Add the authorization header to the request based on these things
                _httpRequestMessageAuthorizer.AddAuthorization(request, credential, nonce, unixTime);
            }

            // Fire off the request and wait for the response
            var response = await _client.SendAsync(request);

            // Return the response
            return response;
        }

        #endregion

        #region Public methods

        public void Dispose() => _client.Dispose();

        public ICredentialController GetCredentialController() => throw new NotImplementedException();

        public IManifestsController GetManifestsController() => new RemoteManifestsController(SendRequestAsync);

        public IOwnerController GetOwnerController() => throw new NotImplementedException();

        #endregion
    }
}

namespace Client.Networking.Http
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using DistributedStorage.Authentication;
    using DistributedStorage.Authorization;

    public sealed class HttpRequestMessageAuthorizer
    {
        private readonly RequestToAuthorizationTokenFactory _authorizationTokenFactory;
        private readonly StringAndAuthorizationTokenAdapter _stringAndAuthorizationTokenAdapter;

        public HttpRequestMessageAuthorizer(RequestToAuthorizationTokenFactory authorizationTokenFactory, StringAndAuthorizationTokenAdapter stringAndAuthorizationTokenAdapter)
        {
            _authorizationTokenFactory = authorizationTokenFactory;
            _stringAndAuthorizationTokenAdapter = stringAndAuthorizationTokenAdapter;
        }

        /// <summary>
        /// Sets the "Authorization" header of the given <paramref name="requestMessage"/> to a string that can be recognized by the "MATT" authentication scheme
        /// </summary>
        public void AddAuthorization(HttpRequestMessage requestMessage, Credential credential, byte[] nonce, long unixTime)
        {
            var adapter = new HttpRequestMessageToRequestAdapter(requestMessage);
            var token = _authorizationTokenFactory.CreateTokenFor(adapter, credential, nonce, unixTime);
            var authString = _stringAndAuthorizationTokenAdapter.CreateFromToken(token);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("MATT", authString);
        }
    }
}

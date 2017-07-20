using DistributedStorage.Authorization;

namespace Server.Models.Authentication.Schemes
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using Authorization;
    using DistributedStorage.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Networking.Http;

    public sealed class MattHttpRequestAuthenticationScheme : IHttpRequestAuthenticationScheme
    {
        private const string ConstAuthenticationType = "MATT";
        private static readonly Regex Regex = new Regex($@"^{ConstAuthenticationType} (.*)$");

        private readonly SecretRepository _secretRepository;
        private readonly StringToAuthorizationTokenAdapter _stringToAuthorizationTokenAdapter;
        private readonly RequestToAuthorizationTokenFactory _requestToAuthorizationTokenFactory;
        private readonly TimeSpan _maxSkew;
        private readonly ReplayDetector<string> _replayDetector;

        public string AuthenticationType => ConstAuthenticationType;

        public MattHttpRequestAuthenticationScheme(
            SecretRepository secretRepository,
            StringToAuthorizationTokenAdapter stringToAuthorizationTokenAdapter,
            RequestToAuthorizationTokenFactory requestToAuthorizationTokenFactory,
            TimeSpan replayAttentionSpan
            )
        {
            _secretRepository = secretRepository;
            _stringToAuthorizationTokenAdapter = stringToAuthorizationTokenAdapter;
            _requestToAuthorizationTokenFactory = requestToAuthorizationTokenFactory;
            _replayDetector = new ReplayDetector<string>(replayAttentionSpan); // Note we don't dispose of the replay detector, because middleware will be alive for the life of the web application
            _maxSkew = new TimeSpan(replayAttentionSpan.Ticks / 2); // We cut it in half so that the replay detector can catch tokens provided both this far in the future and this far in the past
        }

        public bool TryAuthenticate(HttpRequest request, out Credential credential)
        {
            credential = null;

            // Make sure there is an Authorization header
            if (!request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaderValues))
                return false;

            // Make sure they provided a value for the Authorization header
            var authorizationHeaderValue = authorizationHeaderValues.FirstOrDefault();
            if (authorizationHeaderValue == null)
                return false;

            // Make sure the authorization header value matches the scheme we're looking for
            var match = Regex.Match(authorizationHeaderValue);
            if (!match.Success)
                return false; // The auth header format isn't right
            var tokenString = match.Groups[1].Value;

            // Try to get an authorization token from the authorization header value
            if (!_stringToAuthorizationTokenAdapter.TryCreateFromString(tokenString, out var tokenProvidedByClient))
                return false;

            // Make sure the time isn't too far out of whack
            var timeFromTokenProvidedByClient = DateTimeOffset.FromUnixTimeSeconds(tokenProvidedByClient.UnixTime);
            var skew = DateTimeOffset.Now - timeFromTokenProvidedByClient;
            if (skew.Duration() > _maxSkew)
                return false; // The time given by the provided token is too different from our time

            // Make sure we haven't seen this token's nonce before
            if (!_replayDetector.IsUnique(Convert.ToBase64String(tokenProvidedByClient.Nonce)))
                return false; // We have seen this nonce before

            // Figure out the client's corresponding secret
            byte[] secret; // The corresponding client secret
            using (var hmacer = new HMACSHA256(_secretRepository.GetCachedSecret()))
            {
                secret = hmacer.ComputeHash(tokenProvidedByClient.Id);
            }
            // Turn the client's ID and secret into a Credential
            var clientCredentialGeneratedByServer = new Credential(tokenProvidedByClient.Id, secret);

            // Adapt the request into the interface required for the authorization token factory
            var requestAdapter = new HttpRequestToRequestAdapter(request);

            // Let's create a new authorization token using the information given by the client's token (and using the client's secret), and see if we come up with the same HMAC
            var tokenGeneratedByServer = _requestToAuthorizationTokenFactory.CreateTokenFor(requestAdapter, clientCredentialGeneratedByServer, tokenProvidedByClient.Nonce, tokenProvidedByClient.UnixTime);
            if (!tokenGeneratedByServer.Hmac.SequenceEqual(tokenProvidedByClient.Hmac))
                return false; // The HMACs don't match

            // If we made it this far, then the client's token checks out: they authorized this request and we know who they are
            credential = clientCredentialGeneratedByServer;
            return true;
        }
    }
}

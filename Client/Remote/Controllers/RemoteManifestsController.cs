using Microsoft.Net.Http.Headers;

namespace Client.Remote.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using DistributedStorage.Authentication;
    using DistributedStorage.Authorization;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;

    public sealed class RemoteManifestsController : IManifestsController
    {
        private readonly Credential _credential;

        public RemoteManifestsController(Credential credential)
        {
            _credential = credential;
        }

        public IReadOnlyList<string> GetManifestIds()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Matt's distributed storage client");


            var request = new HttpRequestMessage(HttpMethod.Get, "/manifests");
            RequestToAuthorizationTokenFactory a;
            var token = a.CreateTokenFor(request, _credential, nonce, unixTime);
            throw new NotImplementedException();
        }

        public bool TryAddNewManifest(Manifest manifest) => throw new NotImplementedException();

        public bool TryDeleteManifest(string manifestId) => throw new NotImplementedException();

        public bool TryGetManifest(string manifestId, out Manifest manifest) => throw new NotImplementedException();

        public bool TryAddNewSlice(string manifestId, Slice slice) => throw new NotImplementedException();

        public bool TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds) => throw new NotImplementedException();

        public bool TryDeleteSlice(string manifestId, string sliceId) => throw new NotImplementedException();

        public bool TryGetSlice(string manifestId, string sliceId, out Slice slice) => throw new NotImplementedException();
    }
}

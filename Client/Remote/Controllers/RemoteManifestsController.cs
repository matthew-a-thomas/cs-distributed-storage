namespace Client.Remote.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;

    public sealed class RemoteManifestsController : IManifestsController
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendRequestAsyncDelegate;

        public RemoteManifestsController(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendRequestAsyncDelegate)
        {
            _sendRequestAsyncDelegate = sendRequestAsyncDelegate;
        }

        public IReadOnlyList<string> GetManifestIds()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/manifests");
            _sendRequestAsyncDelegate(request);
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

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

        public Task<IReadOnlyList<string>> GetManifestIdsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/manifests");
            _sendRequestAsyncDelegate(request);
            throw new NotImplementedException();
        }

        public Task AddNewManifestAsync(Manifest manifest) => throw new NotImplementedException();

        public Task DeleteManifestAsync(string manifestId) => throw new NotImplementedException();

        public Task<Manifest> GetManifestAsync(string manifestId) => throw new NotImplementedException();

        public Task AddNewSliceAsync(string manifestId, Slice slice) => throw new NotImplementedException();

        public Task<IReadOnlyList<string>> GetSliceIdsAsync(string manifestId) => throw new NotImplementedException();

        public Task DeleteSliceAsync(string manifestId, string sliceId) => throw new NotImplementedException();

        public Task<Slice> GetSliceAsync(string manifestId, string sliceId) => throw new NotImplementedException();
    }
}

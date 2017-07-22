namespace Client.Remote.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http;
    using Networking.Http;

    public sealed class RemoteManifestsController : IManifestsController
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendRequestAsyncDelegate;

        public RemoteManifestsController(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendRequestAsyncDelegate)
        {
            _sendRequestAsyncDelegate = sendRequestAsyncDelegate;
        }

        public async Task<StatusResponse<IReadOnlyList<string>>> GetManifestIdsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/manifests");
            var response = await _sendRequestAsyncDelegate(request);
            var statusResponse = await response.GetContentsAsStatusResponseAsync<IReadOnlyList<string>>();
            return statusResponse;
        }

        public Task<HttpStatusCode> AddNewManifestAsync(Manifest manifest) => throw new NotImplementedException();

        public Task<HttpStatusCode> DeleteManifestAsync(string manifestId) => throw new NotImplementedException();

        public Task<StatusResponse<Manifest>> GetManifestAsync(string manifestId) => throw new NotImplementedException();

        public Task<HttpStatusCode> AddNewSliceAsync(string manifestId, Slice slice) => throw new NotImplementedException();

        public Task<StatusResponse<IReadOnlyList<string>>> GetSliceIdsAsync(string manifestId) => throw new NotImplementedException();

        public Task<HttpStatusCode> DeleteSliceAsync(string manifestId, string sliceId) => throw new NotImplementedException();

        public Task<StatusResponse<Slice>> GetSliceAsync(string manifestId, string sliceId) => throw new NotImplementedException();
    }
}

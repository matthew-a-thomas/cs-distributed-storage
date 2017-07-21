namespace Client.Remote.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http.Exceptions;
    using Newtonsoft.Json;

    public sealed class RemoteManifestsController : IManifestsController
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _sendRequestAsyncDelegate;

        public RemoteManifestsController(Func<HttpRequestMessage, Task<HttpResponseMessage>> sendRequestAsyncDelegate)
        {
            _sendRequestAsyncDelegate = sendRequestAsyncDelegate;
        }

        public async Task<IReadOnlyList<string>> GetManifestIdsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/manifests");
            var response = await _sendRequestAsyncDelegate(request);
            if (!response.IsSuccessStatusCode)
                throw HttpException.GenerateException(response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<IReadOnlyList<string>>(responseString);
            return list;
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

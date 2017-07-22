namespace DistributedStorage.Networking.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Encoding;
    using Http;

    public interface IManifestsController
    {
        Task<StatusResponse<IReadOnlyList<string>>> GetManifestIdsAsync();
        Task<HttpStatusCode> AddNewManifestAsync(Manifest manifest);
        Task<HttpStatusCode> DeleteManifestAsync(string manifestId);
        Task<StatusResponse<Manifest>> GetManifestAsync(string manifestId);
        Task<HttpStatusCode> AddNewSliceAsync(string manifestId, Slice slice);
        Task<StatusResponse<IReadOnlyList<string>>> GetSliceIdsAsync(string manifestId);
        Task<HttpStatusCode> DeleteSliceAsync(string manifestId, string sliceId);
        Task<StatusResponse<Slice>> GetSliceAsync(string manifestId, string sliceId);
    }
}

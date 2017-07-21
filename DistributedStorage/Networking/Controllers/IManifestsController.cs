namespace DistributedStorage.Networking.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Encoding;

    public interface IManifestsController
    {
        Task<IReadOnlyList<string>> GetManifestIdsAsync();
        Task AddNewManifestAsync(Manifest manifest);
        Task DeleteManifestAsync(string manifestId);
        Task<Manifest> GetManifestAsync(string manifestId);
        Task AddNewSliceAsync(string manifestId, Slice slice);
        Task<IReadOnlyList<string>> GetSliceIdsAsync(string manifestId);
        Task DeleteSliceAsync(string manifestId, string sliceId);
        Task<Slice> GetSliceAsync(string manifestId, string sliceId);
    }
}

namespace DistributedStorage.Networking.Controllers
{
    using System.Collections.Generic;
    using Encoding;

    public interface IManifestsController
    {
        IReadOnlyList<string> GetManifestIds();
        bool TryAddNewManifest(Manifest manifest);
        bool TryDeleteManifest(string manifestId);
        bool TryGetManifest(string manifestId, out Manifest manifest);
        bool TryAddNewSlice(string manifestId, Slice slice);
        bool TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds);
        bool TryDeleteSlice(string manifestId, string sliceId);
        bool TryGetSlice(string manifestId, string sliceId, out Slice slice);
    }
}

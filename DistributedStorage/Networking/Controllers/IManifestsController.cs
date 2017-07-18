namespace DistributedStorage.Networking.Controllers
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using Encoding;

    public interface IManifestsController
    {
        IReadOnlyList<string> GetManifestIds(IIdentity userIdentity);
        bool TryAddNewManifest(IIdentity userIdentity, Manifest manifest);
        bool TryDeleteManifest(IIdentity userIdentity, string manifestId);
        bool TryGetManifest(string manifestId, out Manifest manifest);
        bool TryAddNewSlice(IIdentity userIdentity, string manifestId, Slice slice);
        bool TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds);
        bool TryDeleteSlice(IIdentity userIdentity, string manifestId, string sliceId);
        bool TryGetSlice(IIdentity userIdentity, string manifestId, string sliceId, out Slice slice);
    }
}

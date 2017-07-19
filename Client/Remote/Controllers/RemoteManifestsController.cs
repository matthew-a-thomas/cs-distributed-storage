namespace Client.Remote.Controllers
{
    using System.Collections.Generic;
    using DistributedStorage.Authentication;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;

    public sealed class RemoteManifestsController : IManifestsController
    {
        private readonly Credential _credential;

        public RemoteManifestsController(Credential credential)
        {
            _credential = credential;
        }

        public IReadOnlyList<string> GetManifestIds() => throw new System.NotImplementedException();

        public bool TryAddNewManifest(Manifest manifest) => throw new System.NotImplementedException();

        public bool TryDeleteManifest(string manifestId) => throw new System.NotImplementedException();

        public bool TryGetManifest(string manifestId, out Manifest manifest) => throw new System.NotImplementedException();

        public bool TryAddNewSlice(string manifestId, Slice slice) => throw new System.NotImplementedException();

        public bool TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds) => throw new System.NotImplementedException();

        public bool TryDeleteSlice(string manifestId, string sliceId) => throw new System.NotImplementedException();

        public bool TryGetSlice(string manifestId, string sliceId, out Slice slice) => throw new System.NotImplementedException();
    }
}

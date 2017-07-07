namespace AspNet.Models.Manifests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage.Containers;
    using Slices;

    /// <summary>
    /// An implementation of <see cref="IManifestRepository"/> that adapts an <see cref="IFactoryContainer{TKey, TValue}"/>
    /// </summary>
    public class ManifestContainerToManifestRepositoryAdapter : IManifestRepository
    {
        private readonly IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> _manifestContainer;

        public ManifestContainerToManifestRepositoryAdapter(IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> manifestContainer) => _manifestContainer = manifestContainer;

        public IEnumerable<string> ListManifestIds() => _manifestContainer.GetKeys().Select(manifest => manifest.ToString());
        public bool TryAddManifest(Manifest manifest) => _manifestContainer.TryCreate(manifest, out _);
        public bool TryDeleteManifestWithId(string id) => TryGetManifestWithId(id, out var manifest) && _manifestContainer.TryRemove(manifest);
        public bool TryGetManifestWithId(string id, out Manifest manifest)
        {
            manifest = _manifestContainer.GetKeys().FirstOrDefault(x => x.ToString().Equals(id, StringComparison.OrdinalIgnoreCase));
            return manifest != null;
        }
        public bool TryGetSliceRepositoryFor(Manifest manifest, out ISliceRepository sliceRepository)
        {
            sliceRepository = null;
            if (!_manifestContainer.TryGet(manifest, out var container))
                return false;
            sliceRepository = new SliceContainerToSliceRepositoryAdapter(container);
            return true;
        }
    }
}

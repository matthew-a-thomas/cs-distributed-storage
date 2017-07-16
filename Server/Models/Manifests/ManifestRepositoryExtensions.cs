namespace Server.Models.Manifests
{
    using Slices;

    public static class ManifestRepositoryExtensions
    {
        public static bool TryGetSliceRepositoryForManifestWithId(this IManifestRepository manifestRepository, string manifestId, out ISliceRepository sliceRepository)
        {
            sliceRepository = null;
            return manifestRepository.TryGetManifestWithId(manifestId, out var manifest) && manifestRepository.TryGetSliceRepositoryFor(manifest, out sliceRepository);
        }
    }
}

namespace Server.Models.Manifests
{
    using System.Collections.Generic;
    using DistributedStorage.Encoding;
    using Slices;

    /// <summary>
    /// A repository for <see cref="Manifest"/>s
    /// </summary>
    public interface IManifestRepository
    {
        /// <summary>
        /// Lists all available <see cref="Manifest"/> IDs
        /// </summary>
        IEnumerable<string> ListManifestIds();

        /// <summary>
        /// Tries to add the given <paramref name="manifest"/>
        /// </summary>
        bool TryAddManifest(Manifest manifest);

        /// <summary>
        /// Tries to delete things associated with the <see cref="Manifest"/> having the given <paramref name="id"/>
        /// </summary>
        bool TryDeleteManifestWithId(string id);

        /// <summary>
        /// Tries to retrieve the <see cref="Manifest"/> having the given <paramref name="id"/>
        /// </summary>
        bool TryGetManifestWithId(string id, out Manifest manifest);

        /// <summary>
        /// Tries to get/create an <see cref="ISliceRepository"/> for the given <paramref name="manifest"/>
        /// </summary>
        bool TryGetSliceRepositoryFor(Manifest manifest, out ISliceRepository sliceRepository);
    }
}
namespace AspNet.Controllers
{
    using System;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage.Containers;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ManifestsController : Controller
    {
        /// <summary>
        /// The container for manifests
        /// </summary>
        private readonly IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> _manifestsContainer;

        /// <summary>
        /// Creates a new <see cref="ManifestsController"/> using the given <see cref="Manifest"/> container
        /// </summary>
        public ManifestsController(IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> manifestsContainer)
        {
            _manifestsContainer = manifestsContainer;
        }

        /// <summary>
        /// Retrieves a listing of all manifest hashes
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get() => _manifestsContainer.GetKeys().Select(manifest => manifest.ToString());

        /// <summary>
        /// Retrieves the <see cref="Models.Manifest"/> that corresponds to the given <paramref name="id"/>, if one exists.
        /// Otherwise, null is returned.
        /// </summary>
        [HttpGet("{id}")]
        public Models.Manifest Get(string id)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery <-- R# is wrong, and doesn't know that the "out" parameter will kill a Linq query
            foreach (var manifest in _manifestsContainer.GetKeys())
            {
                if (!manifest.ToString().Equals(id, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!_manifestsContainer.TryGet(manifest, out var container))
                    continue;
                return new Models.Manifest
                {
                    Id = manifest.ToString(),
                    SliceIds = container.GetKeys().Select(hash => hash.ToString()).ToArray()
                };
            }
            return null;
        }
    }
}
namespace AspNet.Controllers
{
    using Models;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Storage.Containers;
    using Manifest = Models.Manifest;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ManifestsController : Controller
    {
        /// <summary>
        /// A listing of manifests
        /// </summary>
        private readonly IFactoryContainer<DistributedStorage.Encoding.Manifest, IAddableContainer<Hash, DistributedStorage.Encoding.Slice>> _manifestsContainer;

        /// <summary>
        /// Creates a new <see cref="ManifestsController"/> using the given <see cref="Manifest"/> container
        /// </summary>
        public ManifestsController(IFactoryContainer<DistributedStorage.Encoding.Manifest, IAddableContainer<Hash, DistributedStorage.Encoding.Slice>> manifestsContainer)
        {
            _manifestsContainer = manifestsContainer;
        }
        
        /// <summary>
        /// Retrieves a listing of all manifest hashes
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get() =>
            _manifestsContainer
            .GetKeys()
            .Select(manifest => manifest.ToString());

        /// <summary>
        /// Retrieves the <see cref="Models.Manifest"/> that corresponds to the given <paramref name="id"/>, if one exists.
        /// Otherwise, null is returned.
        /// </summary>
        [HttpGet("{id}")]
        public Manifest Get(string id) =>
            _manifestsContainer
            .GetKeys()
            .Where(manifest => manifest.ToString().Equals(id, StringComparison.OrdinalIgnoreCase))
            .Select(manifest =>
            {
                string[] sliceIds = null;
                if (_manifestsContainer.TryGet(manifest, out var container))
                    sliceIds = container.GetKeys().Select(hash => hash.ToString()).ToArray();
                var aspModelManifest = manifest.ToManifest(sliceIds);
                return aspModelManifest;
            })
            .FirstOrDefault();
    }
}
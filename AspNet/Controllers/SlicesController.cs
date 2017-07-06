namespace AspNet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Storage.Containers;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Slice = Models.Slice;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SlicesController : Controller
    {
        private readonly IFactoryContainer<DistributedStorage.Encoding.Manifest, IAddableContainer<Hash, DistributedStorage.Encoding.Slice>> _manifestsContainer;

        public SlicesController(IFactoryContainer<DistributedStorage.Encoding.Manifest, IAddableContainer<Hash, DistributedStorage.Encoding.Slice>> manifestsContainer)
        {
            _manifestsContainer = manifestsContainer;
        }

        /// <summary>
        /// Redirects to the manifests resource listing
        /// </summary>
        [HttpGet]
        public IActionResult Get() => new RedirectResult("manifests");

        /// <summary>
        /// Gets a listing of all slice IDs associated with the given <paramref name="manifestId"/>
        /// </summary>
        [HttpGet("{manifestId}")]
        public IEnumerable<string> Get(string manifestId)
        {
            foreach (var manifest in _manifestsContainer.GetKeys().Where(manifest => manifest.ToString().Equals(manifestId, StringComparison.OrdinalIgnoreCase)))
            {
                if (!_manifestsContainer.TryGet(manifest, out var container))
                    break;
                var sliceIds = container.GetKeys().Select(hash => hash.ToString());
                return sliceIds;
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Retrieves the slice corresponding with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [HttpGet("{manifestId}/{sliceId}")]
        public Slice Get(string manifestId, string sliceId)
        {
            foreach (var manifest in _manifestsContainer.GetKeys().Where(manifest => manifest.ToString().Equals(manifestId, StringComparison.OrdinalIgnoreCase)))
            {
                if (!_manifestsContainer.TryGet(manifest, out var container))
                    return null;
                foreach (var hash in container.GetKeys().Where(hash => hash.ToString().Equals(sliceId, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!container.TryGet(hash, out var slice))
                        return null;
                    var aspModelSlice = slice.ToSlice();
                    return aspModelSlice;
                }
            }
            return null;
        }
    }
}
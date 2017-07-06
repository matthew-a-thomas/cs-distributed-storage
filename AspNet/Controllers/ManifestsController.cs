namespace AspNet.Controllers
{
    using Models;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Manifest = DistributedStorage.Encoding.Manifest;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ManifestsController : Controller
    {
        /// <summary>
        /// A listing of manifests
        /// </summary>
        private readonly IEnumerable<Manifest> _manifestsListing;

        /// <summary>
        /// Creates a new <see cref="ManifestsController"/> using the given <see cref="Manifest"/> listing
        /// </summary>
        public ManifestsController(IEnumerable<Manifest> manifestsListing)
        {
            _manifestsListing = manifestsListing;
        }

        /// <summary>
        /// Retrieves a listing of all manifest hashes
        /// </summary>
        [HttpGet]
        public IEnumerable<string> Get() =>
            _manifestsListing
            .Select(manifest => manifest.ToString());

        /// <summary>
        /// Retrieves the <see cref="Models.Manifest"/> that corresponds to the given <paramref name="id"/>, if one exists.
        /// Otherwise, null is returned.
        /// </summary>
        [HttpGet("{id}")]
        public Models.Manifest Get(string id) =>
            _manifestsListing
            .Where(manifest => manifest.ToString().Equals(id, StringComparison.OrdinalIgnoreCase))
            .Select(manifest => manifest.ToManifest())
            .FirstOrDefault();
    }
}
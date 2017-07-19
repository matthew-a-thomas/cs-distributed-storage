namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Authorization.Policies;
    using Models.Manifests;

    [Authorize(OwnerOnlyPolicyFactory.PolicyName)]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ManifestsController : Controller, IManifestsController
    {
        #region Private fields
        
        /// <summary>
        /// Shorter than writing ((IManifestsController)this).something
        /// </summary>
        private readonly IManifestsController _this;

        /// <summary>
        /// Manages <see cref="Manifest"/>s and <see cref="Slice"/>s for us
        /// </summary>
        private readonly IManifestRepository _manifestRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ManifestsController"/> using the given <see cref="IManifestsController"/>
        /// </summary>
        public ManifestsController(IManifestRepository manifestRepository)
        {
            _this = this;
            _manifestRepository = manifestRepository;
        }

        #endregion

        #region /manifests

        /// <summary>
        /// Retrieves a listing of all manifest hashes
        /// </summary>
        [HttpGet]
        public IActionResult GetManifestIds() => new OkObjectResult(_this.GetManifestIds());

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public IActionResult TryAddNewManifest([FromBody] Manifest manifest) => new OkObjectResult(_this.TryAddNewManifest(manifest));

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public IActionResult TryDeleteManifest(string manifestId) => new OkObjectResult(_this.TryDeleteManifest(manifestId));

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}")]
        public IActionResult GetManifest(string manifestId) => _this.TryGetManifest(manifestId, out var manifest) ? (IActionResult)new OkObjectResult(manifest) : new NotFoundResult();

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public IActionResult TryAddNewSlice(string manifestId, [FromBody] Slice slice) => new OkObjectResult(_this.TryAddNewSlice(manifestId, slice));

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices")]
        public IActionResult TryGetSliceIds(string manifestId) => _this.TryGetSliceIds(manifestId, out var sliceIds) ? (IActionResult)new OkObjectResult(sliceIds) : new NotFoundResult();
        
        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public IActionResult TryDeleteSlice(string manifestId, string sliceId) => new OkObjectResult(_this.TryDeleteSlice(manifestId, sliceId));

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public IActionResult GetSlice(string manifestId, string sliceId) => _this.TryGetSlice(manifestId, sliceId, out var slice) ? (IActionResult)new OkObjectResult(slice) : new NotFoundResult();

        #endregion

        #region IManifestsController

        IReadOnlyList<string> IManifestsController.GetManifestIds() => _manifestRepository.ListManifestIds().ToList();

        bool IManifestsController.TryAddNewManifest(Manifest manifest) => _manifestRepository.TryAddManifest(manifest);

        bool IManifestsController.TryDeleteManifest(string manifestId) => _manifestRepository.TryDeleteManifestWithId(manifestId);

        bool IManifestsController.TryGetManifest(string manifestId, out Manifest manifest) => _manifestRepository.TryGetManifestWithId(manifestId, out manifest);

        bool IManifestsController.TryAddNewSlice(string manifestId, Slice slice) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryAddSlice(slice);

        bool IManifestsController.TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds)
        {
            sliceIds = null;
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                return false;
            sliceIds = sliceRepository.ListSliceIds().ToList();
            return true;
        }

        bool IManifestsController.TryDeleteSlice(string manifestId, string sliceId) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryDeleteSliceWithId(sliceId);

        bool IManifestsController.TryGetSlice(string manifestId, string sliceId, out Slice slice)
        {
            slice = null;
            return _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryGetSliceWithId(sliceId, out slice);
        }

        #endregion
    }
}
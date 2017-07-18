namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
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
        #region Private properties

        /// <summary>
        /// Shorter than writing ((IManifestsController)this).whatever
        /// </summary>
        private IManifestsController This => this;

        #endregion

        #region Private fields

        /// <summary>
        /// Manages <see cref="Manifest"/>s and <see cref="Slice"/>s for us
        /// </summary>
        private readonly IManifestRepository _manifestRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ManifestsController"/> using the given <see cref="IManifestRepository"/>
        /// </summary>
        public ManifestsController(IManifestRepository manifestRepository)
        {
            _manifestRepository = manifestRepository;
        }

        #endregion

        #region /manifests

        /// <summary>
        /// Retrieves a listing of all manifest hashes
        /// </summary>
        [HttpGet]
        public IActionResult GetManifestIds() => new OkObjectResult(This.GetManifestIds(User.Identity));
        IReadOnlyList<string> IManifestsController.GetManifestIds(IIdentity userIdentity) => _manifestRepository.ListManifestIds().ToList();

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public IActionResult TryAddNewManifest([FromBody] Manifest manifest) => new OkObjectResult(This.TryAddNewManifest(User.Identity, manifest));
        bool IManifestsController.TryAddNewManifest(IIdentity userIdentity, Manifest manifest) => _manifestRepository.TryAddManifest(manifest);

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public IActionResult TryDeleteManifest(string manifestId) => new OkObjectResult(This.TryDeleteManifest(User.Identity, manifestId));
        bool IManifestsController.TryDeleteManifest(IIdentity userIdentity, string manifestId) => _manifestRepository.TryDeleteManifestWithId(manifestId);

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}")]
        public IActionResult GetManifest(string manifestId) => This.TryGetManifest(manifestId, out var manifest) ? (IActionResult)new OkObjectResult(manifest) : new NotFoundResult();
        bool IManifestsController.TryGetManifest(string manifestId, out Manifest manifest) => _manifestRepository.TryGetManifestWithId(manifestId, out manifest);

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public IActionResult TryAddNewSlice(string manifestId, [FromBody] Slice slice) => new OkObjectResult(This.TryAddNewSlice(User.Identity, manifestId, slice));
        bool IManifestsController.TryAddNewSlice(IIdentity userIdentity, string manifestId, Slice slice) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryAddSlice(slice);

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices")]
        public IActionResult TryGetSliceIds(string manifestId) => This.TryGetSliceIds(manifestId, out var sliceIds) ? (IActionResult)new OkObjectResult(sliceIds) : new NotFoundResult();
        bool IManifestsController.TryGetSliceIds(string manifestId, out IReadOnlyList<string> sliceIds)
        {
            sliceIds = null;
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                return false;
            sliceIds = sliceRepository.ListSliceIds().ToList();
            return true;
        }

        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public IActionResult TryDeleteSlice(string manifestId, string sliceId) => new OkObjectResult(This.TryDeleteSlice(User.Identity, manifestId, sliceId));
        bool IManifestsController.TryDeleteSlice(IIdentity userIdentity, string manifestId, string sliceId) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryDeleteSliceWithId(sliceId);

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public IActionResult GetSlice(string manifestId, string sliceId) => This.TryGetSlice(User.Identity, manifestId, sliceId, out var slice) ? (IActionResult)new OkObjectResult(slice) : new NotFoundResult();
        bool IManifestsController.TryGetSlice(IIdentity userIdentity, string manifestId, string sliceId, out Slice slice)
        {
            slice = null;
            return _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryGetSliceWithId(sliceId, out slice);
        }

        #endregion
    }
}
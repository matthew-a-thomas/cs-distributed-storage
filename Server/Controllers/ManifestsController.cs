namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Authorization.Policies;
    using Models.Manifests;
    using Models.Networking.Http;

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
        public async Task<IActionResult> GetManifestIdsAsync() => (await _this.GetManifestIdsAsync()).ToActionResult();

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddNewManifestAsync([FromBody] Manifest manifest) => (await _this.AddNewManifestAsync(manifest)).ToActionResult();

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public async Task<IActionResult> DeleteManifestAsync(string manifestId) => (await _this.DeleteManifestAsync(manifestId)).ToActionResult();

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}")]
        public async Task<IActionResult> GetManifestAsync(string manifestId) => (await _this.GetManifestAsync(manifestId)).ToActionResult();

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public async Task<IActionResult> AddNewSliceAsync(string manifestId, [FromBody] Slice slice) => (await _this.AddNewSliceAsync(manifestId, slice)).ToActionResult();

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices")]
        public async Task<IActionResult> GetSliceIdsAsync(string manifestId) => (await _this.GetSliceIdsAsync(manifestId)).ToActionResult();
        
        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> DeleteSliceAsync(string manifestId, string sliceId) => (await _this.DeleteSliceAsync(manifestId, sliceId)).ToActionResult();

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> GetSliceAsync(string manifestId, string sliceId) => (await _this.GetSliceAsync(manifestId, sliceId)).ToActionResult();

        #endregion

        #region IManifestsController

        Task<StatusResponse<IReadOnlyList<string>>> IManifestsController.GetManifestIdsAsync() => Task.Run(() => StatusResponse.CreateOk((IReadOnlyList<string>)_manifestRepository.ListManifestIds().ToList()));

        Task<HttpStatusCode> IManifestsController.AddNewManifestAsync(Manifest manifest) => Task.Run(() => !_manifestRepository.TryAddManifest(manifest) ? HttpStatusCode.Conflict : HttpStatusCode.OK);

        Task<HttpStatusCode> IManifestsController.DeleteManifestAsync(string manifestId) => Task.Run(() => !_manifestRepository.TryDeleteManifestWithId(manifestId) ? HttpStatusCode.NotFound : HttpStatusCode.OK);

        Task<StatusResponse<Manifest>> IManifestsController.GetManifestAsync(string manifestId) => Task.Run(() => _manifestRepository.TryGetManifestWithId(manifestId, out var manifest) ? StatusResponse.CreateOk(manifest) : new StatusResponse<Manifest>(HttpStatusCode.NotFound, manifest));

        Task<HttpStatusCode> IManifestsController.AddNewSliceAsync(string manifestId, Slice slice) => Task.Run(() => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) ? (sliceRepository.TryAddSlice(slice) ? HttpStatusCode.OK : HttpStatusCode.Conflict) : HttpStatusCode.NotFound);

        Task<StatusResponse<IReadOnlyList<string>>> IManifestsController.GetSliceIdsAsync(string manifestId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                return new StatusResponse<IReadOnlyList<string>>(HttpStatusCode.NotFound, null);
            IReadOnlyList<string> sliceIds = sliceRepository.ListSliceIds().ToList();
            return StatusResponse.CreateOk(sliceIds);
        });

        Task<HttpStatusCode> IManifestsController.DeleteSliceAsync(string manifestId, string sliceId) => Task.Run(() => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) ? (sliceRepository.TryDeleteSliceWithId(sliceId) ? HttpStatusCode.OK : HttpStatusCode.NotFound) : HttpStatusCode.NotFound);

        Task<StatusResponse<Slice>> IManifestsController.GetSliceAsync(string manifestId, string sliceId) => Task.Run(() => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) ? (sliceRepository.TryGetSliceWithId(sliceId, out var slice) ? StatusResponse.CreateOk(slice) : new StatusResponse<Slice>(HttpStatusCode.NotFound, slice)) : new StatusResponse<Slice>(HttpStatusCode.NotFound, null));

        #endregion
    }
}
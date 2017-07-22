namespace Server.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http.Exceptions;
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
        public async Task<IActionResult> GetManifestIdsAsync() => await FuncExtensions.ToActionResultAsync(_this.GetManifestIdsAsync);

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddNewManifestAsync([FromBody] Manifest manifest) => await FuncExtensions.ToActionResultAsync(() => _this.AddNewManifestAsync(manifest));

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public async Task<IActionResult> DeleteManifestAsync(string manifestId) => await FuncExtensions.ToActionResultAsync(() => _this.DeleteManifestAsync(manifestId));

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}")]
        public async Task<IActionResult> GetManifestAsync(string manifestId) => await FuncExtensions.ToActionResultAsync(() => _this.GetManifestAsync(manifestId));

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public async Task<IActionResult> AddNewSliceAsync(string manifestId, [FromBody] Slice slice) => await FuncExtensions.ToActionResultAsync(() => _this.AddNewSliceAsync(manifestId, slice));

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices")]
        public async Task<IActionResult> GetSliceIdsAsync(string manifestId) => await FuncExtensions.ToActionResultAsync(() => _this.GetSliceIdsAsync(manifestId));
        
        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> DeleteSliceAsync(string manifestId, string sliceId) => await FuncExtensions.ToActionResultAsync(() => _this.DeleteSliceAsync(manifestId, sliceId));

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> GetSliceAsync(string manifestId, string sliceId) => await FuncExtensions.ToActionResultAsync(() => _this.GetSliceAsync(manifestId, sliceId));

        #endregion

        #region IManifestsController

        Task<IReadOnlyList<string>> IManifestsController.GetManifestIdsAsync() => Task.Run(() => (IReadOnlyList<string>)_manifestRepository.ListManifestIds().ToList());

        Task IManifestsController.AddNewManifestAsync(Manifest manifest) => Task.Run(() =>
        {
            if (!_manifestRepository.TryAddManifest(manifest))
                throw HttpException.GenerateException(HttpStatusCode.Conflict);
        });

        Task IManifestsController.DeleteManifestAsync(string manifestId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryDeleteManifestWithId(manifestId))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
        });

        Task<Manifest> IManifestsController.GetManifestAsync(string manifestId) => Task.Run(() => _manifestRepository.TryGetManifestWithId(manifestId, out var manifest) ? manifest : throw HttpException.GenerateException(HttpStatusCode.NotFound));

        Task IManifestsController.AddNewSliceAsync(string manifestId, Slice slice) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
            if (!sliceRepository.TryAddSlice(slice))
                throw HttpException.GenerateException(HttpStatusCode.Conflict);
        });

        Task<IReadOnlyList<string>> IManifestsController.GetSliceIdsAsync(string manifestId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
            IReadOnlyList<string> sliceIds = sliceRepository.ListSliceIds().ToList();
            return sliceIds;
        });

        Task IManifestsController.DeleteSliceAsync(string manifestId, string sliceId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
            if (!sliceRepository.TryDeleteSliceWithId(sliceId))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
        });

        Task<Slice> IManifestsController.GetSliceAsync(string manifestId, string sliceId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
            if (!sliceRepository.TryGetSliceWithId(sliceId, out var slice))
                throw HttpException.GenerateException(HttpStatusCode.NotFound);
            return slice;
        });

        #endregion
    }
}
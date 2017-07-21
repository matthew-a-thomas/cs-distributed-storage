namespace Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http.Exceptions;
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
        public async Task<IActionResult> GetManifestIdsAsync() => await ToActionResult(_this.GetManifestIdsAsync);

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddNewManifestAsync([FromBody] Manifest manifest) => await ToActionResult(() => _this.AddNewManifestAsync(manifest));

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public async Task<IActionResult> DeleteManifestAsync(string manifestId) => await ToActionResult(() => _this.DeleteManifestAsync(manifestId));

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}")]
        public async Task<IActionResult> GetManifestAsync(string manifestId) => await ToActionResult(() => _this.GetManifestAsync(manifestId));

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public async Task<IActionResult> AddNewSliceAsync(string manifestId, [FromBody] Slice slice) => await ToActionResult(() => _this.AddNewSliceAsync(manifestId, slice));

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices")]
        public async Task<IActionResult> GetSliceIdsAsync(string manifestId) => await ToActionResult(() => _this.GetSliceIdsAsync(manifestId));
        
        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> DeleteSliceAsync(string manifestId, string sliceId) => await ToActionResult(() => _this.DeleteSliceAsync(manifestId, sliceId));

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public async Task<IActionResult> GetSliceAsync(string manifestId, string sliceId) => await ToActionResult(() => _this.GetSliceAsync(manifestId, sliceId));

        #endregion

        #region IManifestsController

        Task<IReadOnlyList<string>> IManifestsController.GetManifestIdsAsync() => Task.Run(() => (IReadOnlyList<string>)_manifestRepository.ListManifestIds().ToList());

        Task IManifestsController.AddNewManifestAsync(Manifest manifest) => Task.Run(() =>
        {
            if (!_manifestRepository.TryAddManifest(manifest))
                throw new ConflictException();
        });

        Task IManifestsController.DeleteManifestAsync(string manifestId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryDeleteManifestWithId(manifestId))
                throw new NotFoundException();
        });

        Task<Manifest> IManifestsController.GetManifestAsync(string manifestId) => Task.Run(() => _manifestRepository.TryGetManifestWithId(manifestId, out var manifest) ? manifest : throw new NotFoundException());

        Task IManifestsController.AddNewSliceAsync(string manifestId, Slice slice) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw new NotFoundException();
            if (!sliceRepository.TryAddSlice(slice))
                throw new ConflictException();
        });

        Task<IReadOnlyList<string>> IManifestsController.GetSliceIdsAsync(string manifestId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw new NotFoundException();
            IReadOnlyList<string> sliceIds = sliceRepository.ListSliceIds().ToList();
            return sliceIds;
        });

        Task IManifestsController.DeleteSliceAsync(string manifestId, string sliceId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw new NotFoundException();
            if (!sliceRepository.TryDeleteSliceWithId(sliceId))
                throw new NotFoundException();
        });

        Task<Slice> IManifestsController.GetSliceAsync(string manifestId, string sliceId) => Task.Run(() =>
        {
            if (!_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository))
                throw new NotFoundException();
            if (!sliceRepository.TryGetSliceWithId(sliceId, out var slice))
                throw new NotFoundException();
            return slice;
        });

        #endregion

        #region Static methods

        private static async Task<IActionResult> ToActionResult<T>(Func<Task<T>> asyncFunc)
        {
            try
            {
                var result = await asyncFunc();
                return new OkObjectResult(result);
            }
            catch (HttpException e)
            {
                return new StatusCodeResult(e.HttpStatusCode);
            }
        }

        private static async Task<IActionResult> ToActionResult(Func<Task> asyncFunc)
        {
            try
            {
                await asyncFunc();
                return new OkResult();
            }
            catch (HttpException e)
            {
                return new StatusCodeResult(e.HttpStatusCode);
            }
        }

        #endregion
    }
}
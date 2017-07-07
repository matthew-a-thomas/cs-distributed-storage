namespace AspNet.Controllers
{
    using System.Linq;
    using DistributedStorage.Encoding;
    using Microsoft.AspNetCore.Mvc;
    using Models.Manifests;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ManifestsController : Controller
    {
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
        public IActionResult GetManifestIds() => new OkObjectResult(_manifestRepository.ListManifestIds().ToList());

        /// <summary>
        /// Creates a new container for the given <paramref name="manifest"/>
        /// </summary>
        [HttpPost]
        public IActionResult AddNewManifest([FromBody] Manifest manifest) => new OkObjectResult(_manifestRepository.TryAddManifest(manifest));

        #endregion

        #region /manifests/{manifest ID}

        /// <summary>
        /// Tries to delete the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpDelete("{manifestId}")]
        public IActionResult DeleteManifest(string manifestId) => new OkObjectResult(_manifestRepository.TryDeleteManifestWithId(manifestId));

        /// <summary>
        /// Get the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpGet("{manifestId}")]
        public IActionResult GetManifest(string manifestId) => _manifestRepository.TryGetManifestWithId(manifestId, out var manifest) ? (IActionResult)new OkObjectResult(manifest) : new NotFoundResult();

        #endregion

        #region /manifests/{manifest ID}/slices

        /// <summary>
        /// Tries to add a new <see cref="Slice"/> to associate with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpPost("{manifestId}/slices")]
        public IActionResult AddNewSlice(string manifestId, [FromBody] Slice slice) => new OkObjectResult(_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryAddSlice(slice));

        /// <summary>
        /// Gets a listing of all <see cref="Slice"/> IDs associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>
        /// </summary>
        [HttpGet("{manifestId}/slices")]
        public IActionResult GetSliceIds(string manifestId) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) ? (IActionResult)new OkObjectResult(sliceRepository.ListSliceIds().ToList()) : new NotFoundResult();

        #endregion

        #region /manifests/{manifest ID}/slices/{slice ID}

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> associated with the <see cref="Manifest"/> having the given <paramref name="manifestId"/>, and which has the given <paramref name="sliceId"/>
        /// </summary>
        [HttpDelete("{manifestId}/slices/{sliceId}")]
        public IActionResult DeleteSlice(string manifestId, string sliceId) => new OkObjectResult(_manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryDeleteSliceWithId(sliceId));

        /// <summary>
        /// Gets the <see cref="Slice"/> associated with the given <paramref name="manifestId"/> and having the given <paramref name="sliceId"/>
        /// </summary>
        [HttpGet("{manifestId}/slices/{sliceId}")]
        public IActionResult GetSlice(string manifestId, string sliceId) => _manifestRepository.TryGetSliceRepositoryForManifestWithId(manifestId, out var sliceRepository) && sliceRepository.TryGetSliceWithId(sliceId, out var slice) ? (IActionResult)new OkObjectResult(slice) : new NotFoundResult();

        #endregion
    }
}
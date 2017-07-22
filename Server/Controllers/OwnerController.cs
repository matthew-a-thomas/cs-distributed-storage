namespace Server.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Authorization.Policies;
    using Models.Networking.Http;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OwnerController : Controller, IOwnerController
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly OwnerRepository _ownerRepository;
        private readonly IOwnerController _this;

        public OwnerController(
            OwnerRepository ownerRepository,
            IAuthorizationService authorizationService
            )
        {
            _authorizationService = authorizationService;
            _ownerRepository = ownerRepository;
            _this = this;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwner() => (await _this.GetOwnerAsync()).ToActionResult();

        [HttpPut]
        public async Task<IActionResult> PutOwner([FromBody] string owner) => (await _this.PutOwnerAsync(owner)).ToActionResult();

        Task<StatusResponse<string>> IOwnerController.GetOwnerAsync() => Task.Run(() => new StatusResponse<string>(_ownerRepository.TryGetOwner(out var owner) ? HttpStatusCode.OK : HttpStatusCode.NotFound, owner));

        async Task<StatusResponse<bool>> IOwnerController.PutOwnerAsync(string owner)
        {
            // See if the current owner has already been set and the current user isn't authorized to replace it
            if (_ownerRepository.TryGetOwner(out _) && !await _authorizationService.AuthorizeAsync(User, OwnerOnlyPolicyFactory.PolicyName))
                return new StatusResponse<bool>(HttpStatusCode.Unauthorized, false); // The current user isn't authorized to replace the current owner

            // The current owner hasn't yet been set so anyone can initialize it, or the current user is authorized to replace the current owner
            var didSet = _ownerRepository.TrySetOwner(owner); // Try setting it
            return StatusResponse.CreateOk(didSet);
        }
    }
}
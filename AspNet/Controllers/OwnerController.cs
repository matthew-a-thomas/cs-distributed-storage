namespace AspNet.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Authorization.Policies;
    
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OwnerController : Controller
    {
        private readonly OwnerRepository _ownerRepository;
        private readonly IAuthorizationService _authorizationService;

        public OwnerController(
            OwnerRepository ownerRepository,
            IAuthorizationService authorizationService
            )
        {
            _ownerRepository = ownerRepository;
            _authorizationService = authorizationService;
        }
        
        [HttpGet]
        public IActionResult GetOwner() => _ownerRepository.TryGetOwner(out var currentOwner) ? (IActionResult) new OkObjectResult(currentOwner) : new NotFoundResult();
        
        [HttpPut]
        public async Task<IActionResult> PutOwner([FromBody] Owner owner)
        {
            // See if the current owner has already been set and the current user isn't authorized to replace it
            if (_ownerRepository.TryGetOwner(out _) && !await _authorizationService.AuthorizeAsync(User, OwnerOnlyPolicyFactory.PolicyName))
                return new UnauthorizedResult(); // The current user isn't authorized to replace the current owner

            // The current owner hasn't yet been set so anyone can initialize it, or the current user is authorized to replace the current owner
            var didSet = _ownerRepository.TrySetOwner(owner); // Try setting it
            return new OkObjectResult(didSet);
        }
    }
}
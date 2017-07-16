namespace Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models.Authentication;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CredentialController : Controller
    {
        private readonly CredentialFactory _credentialFactory;

        public CredentialController(CredentialFactory credentialFactory)
        {
            _credentialFactory = credentialFactory;
        }

        [HttpGet]
        public IActionResult GenerateToken()
        {
            var credential = _credentialFactory.CreateNewCredential();
            return credential == null ? (IActionResult) new NotFoundResult() : new OkObjectResult(credential);
        }
    }
}

namespace Server.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Authentication;
    using Models.Networking.Http;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CredentialController : Controller, ICredentialController
    {
        private readonly CredentialFactory _credentialFactory;
        private readonly ICredentialController _this;

        public CredentialController(CredentialFactory credentialFactory)
        {
            _credentialFactory = credentialFactory;
            _this = this;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateTokenAsync() => (await _this.GenerateCredentialAsync()).ToActionResult();

        Task<StatusResponse<Credential>> ICredentialController.GenerateCredentialAsync() => Task.Run(() =>
        {
            var credential = _credentialFactory.CreateNewCredential();
            return new StatusResponse<Credential>(credential == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, credential);
        });
    }
}

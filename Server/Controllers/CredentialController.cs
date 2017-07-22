namespace Server.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Controllers;
    using DistributedStorage.Networking.Http.Exceptions;
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
        public Task<IActionResult> GenerateTokenAsync() => FuncExtensions.ToActionResultAsync(_this.GenerateCredentialAsync);

        Task<Credential> ICredentialController.GenerateCredentialAsync() => Task.Run(() =>
        {
            var credential = _credentialFactory.CreateNewCredential();
            return credential ?? throw HttpException.GenerateException(HttpStatusCode.NotFound);
        });
    }
}

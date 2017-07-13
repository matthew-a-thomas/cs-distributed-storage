namespace AspNet.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly TokenFactory _tokenFactory;

        public TokenController(TokenFactory tokenFactory)
        {
            _tokenFactory = tokenFactory;
        }

        [HttpGet]
        public IActionResult GenerateToken()
        {
            var token = _tokenFactory.GenerateNewToken();
            return token == null ? (IActionResult) new NotFoundResult() : new OkObjectResult(token);
        }
    }
}

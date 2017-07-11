namespace AspNet.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {


        [HttpGet("{secret}")]
        public IActionResult GenerateToken(string secret)
        {

        }
    }
}

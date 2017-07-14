namespace AspNet.Models.Authentication.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Middlewares;

    public sealed class AuthenticationMiddleware : MiddlewareBase
    {
        private readonly SecretRepository _secretRepository;

        public AuthenticationMiddleware(
            RequestDelegate next,
            SecretRepository secretRepository
            ) : base(next)
        {
            _secretRepository = secretRepository;
        }

        public override async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaderValues))
                    return;

                foreach (var authorizationHeaderValue in authorizationHeaderValues)
                {
                    
                }
            }
            finally
            {
                await Next(context);
            }
        }
    }
}

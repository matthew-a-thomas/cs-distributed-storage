namespace Server.Models.Authentication.Middleware
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Middlewares;
    using Schemes;

    public sealed class AuthenticationMiddleware : MiddlewareBase
    {
        private readonly IHttpRequestAuthenticationScheme _httpRequestAuthenticationScheme;
        private readonly CredentialToClaimsIdentityAdapter _credentialToClaimsIdentityAdapter;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IHttpRequestAuthenticationScheme httpRequestAuthenticationScheme,
            CredentialToClaimsIdentityAdapter credentialToClaimsIdentityAdapter
            ) : base(next)
        {
            _httpRequestAuthenticationScheme = httpRequestAuthenticationScheme;
            _credentialToClaimsIdentityAdapter = credentialToClaimsIdentityAdapter;
        }

        private void InvokeInner(HttpContext context)
        {
            // Try authenticating them
            if (!_httpRequestAuthenticationScheme.TryAuthenticate(context.Request, out var authenticatedClientCredential))
                return; // We couldn't authenticate them

            // If we made it this far, then the client's token checks out: they authorized it and we know who they are.
            // Therefore, we can create an identity for them.
            var clientIdentity = _credentialToClaimsIdentityAdapter.CreateIdentityFor(authenticatedClientCredential, _httpRequestAuthenticationScheme.AuthenticationType);

            // Give the client's user this identity
            context.User.AddIdentity(clientIdentity);
        }

        public override async Task Invoke(HttpContext context)
        {
            InvokeInner(context);
            await Next(context);
        }
    }
}

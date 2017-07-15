namespace AspNet.Models.Authentication.Middleware
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Middlewares;

    public sealed class AuthenticationMiddleware : MiddlewareBase
    {
        private readonly Regex _regex = new Regex(@"^MATT ([^:]+):([^:]+):([^:]+):(.+)$");
        //                                                ^ ID    ^ nonce ^ time  ^ hmac
        private readonly SecretRepository _secretRepository;

        private readonly TimeSpan _replayAttentionSpan;

        private readonly ReplayDetector<string> _replayDetector;

        public AuthenticationMiddleware(
            RequestDelegate next,
            SecretRepository secretRepository,
            TimeSpan replayAttentionSpan
            ) : base(next)
        {
            _secretRepository = secretRepository;
            _replayAttentionSpan = replayAttentionSpan;
            _replayDetector = new ReplayDetector<string>(replayAttentionSpan);
        }

        public override async Task Invoke(HttpContext context)
        {
            try
            {
                // Make sure there is an Authorization header
                if (!context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaderValues))
                    return;

                // Make sure they provided a value for the Authorization header
                var authorizationHeaderValue = authorizationHeaderValues.FirstOrDefault();
                if (authorizationHeaderValue == null)
                    return;

                // Make sure the value matches our regular expression
                var match = _regex.Match(authorizationHeaderValue);
                if (!match.Success)
                    return;

                // Grab the client's reported ID and request HMAC
                string
                    idBase64 = match.Groups[1].Value,
                    nonceBase64 = match.Groups[2].Value,
                    timeBase64 = match.Groups[3].Value,
                    hmacBase64 = match.Groups[4].Value;
                byte[]
                    id, // The client's reported ID
                    nonce, // A random value sent from the client
                    time, // The client's reported unix time
                    hmac; // The client's reported request HMAC
                try
                {
                    id = Convert.FromBase64String(idBase64);
                    nonce = Convert.FromBase64String(nonceBase64);
                    time = Convert.FromBase64String(timeBase64);
                    hmac = Convert.FromBase64String(hmacBase64);
                }
                catch
                {
                    return;
                }

                // Make sure the time isn't too far out of whack
                throw new NotImplementedException();

                // Figure out the client's corresponding secret
                byte[] secret; // The corresponding client secret
                using (var hmacer = new HMACSHA256(_secretRepository.GetCachedSecret()))
                {
                    secret = hmacer.ComputeHash(id);
                }

                // Let's compute our own HMAC of the request using the client's secret, and see if it matches
                using (var stream = new MemoryStream())
                {
                    foreach (var part in new[]
                    {
                        context.Request.Host.ToString(),
                        context.Request.Method,
                        context.Request.Path.ToString(),
                        context.Request.QueryString.ToString(),
                        context.Request.ContentType
                    })
                    {
                        var bytes = Encoding.UTF8.GetBytes(part);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    stream.Write(nonce, 0, nonce.Length);
                    context.Request.Body.CopyTo(stream);
                }
            }
            finally
            {
                await Next(context);
            }
        }
    }
}

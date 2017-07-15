namespace AspNet.Models.Authorization
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Authentication;
    using Microsoft.AspNetCore.Http;

    public sealed class RequestToAuthorizationTokenFactory
    {
        /// <summary>
        /// Creates an <see cref="AuthorizationToken"/> for the given <paramref name="request"/>,
        /// signifying that the given <paramref name="credential"/> is performing it
        /// </summary>
        public AuthorizationToken CreateTokenFor(HttpRequest request, Credential credential, byte[] nonce, long unixTime)
        {
            using (var stream = new MemoryStream())
            {
                // Copy certain things into the memory stream
                stream.Write(credential.Public, 0, credential.Public.Length); // Write the public part of the credential
                var unixTimeBytes = BitConverter.GetBytes(unixTime);
                stream.Write(unixTimeBytes, 0, unixTimeBytes.Length); // Write the given unix time
                stream.Write(nonce, 0, nonce.Length); // Write the given nonce
                foreach (var part in new[]
                {
                    request.Host.ToString(), // Write the request host name
                    request.Method, // Write the request method
                    request.Path.ToString(), // Write the request path
                    request.QueryString.ToString(), // Write the request query string
                    request.ContentType // Write the request content type
                })
                {
                    var bytes = Encoding.UTF8.GetBytes(part);
                    stream.Write(bytes, 0, bytes.Length);
                }
                request.Body.CopyTo(stream); // Copy the request stream

                // Compute the HMAC of those things using the given credential
                stream.Position = 0;
                using (var hmacer = new HMACSHA256(credential.Private))
                {
                    var hmac = hmacer.ComputeHash(stream);
                    // Create a new token using this HMAC and the other provided things
                    return new AuthorizationToken(credential.Public, hmac, nonce, unixTime);
                }
            }
        }
    }
}

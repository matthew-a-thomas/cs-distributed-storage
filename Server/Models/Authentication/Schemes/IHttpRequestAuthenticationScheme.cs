namespace Server.Models.Authentication.Schemes
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Something which knows how to authenticate <see cref="Credential"/>s from <see cref="HttpRequest"/>s
    /// </summary>
    public interface IHttpRequestAuthenticationScheme
    {
        /// <summary>
        /// Tries to create an authenticated <see cref="Credential"/> from the given <paramref name="request"/>
        /// </summary>
        bool TryAuthenticate(HttpRequest request, out Credential credential);

        /// <summary>
        /// The type of authentication employed by this <see cref="IHttpRequestAuthenticationScheme"/>
        /// </summary>
        string AuthenticationType { get; }
    }
}
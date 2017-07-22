namespace Server.Models.Networking.Http
{
    using System.Collections.Generic;
    using System.IO;
    using DistributedStorage.Networking.Http;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Adapts an <see cref="HttpRequest"/> to an <see cref="IRequest"/>
    /// </summary>
    public sealed class HttpRequestToRequestAdapter : IRequest
    {
        #region Public properties

        public IReadOnlyList<byte> Body
        {
            get
            {
                using (var memoryStream = new MemoryStream())
                {
                    _request.Body.CopyTo(memoryStream);
                    return memoryStream.Length == 0 ? null : memoryStream.ToArray();
                }
            }
        }
        public string ContentType => _request.ContentType;
        public string Host => _request.Host.ToString();
        public string Method => _request.Method;
        public string PathAndQuery => $"{_request.Path}{(_request.QueryString.HasValue ? $"?{_request.QueryString}" : "")}";

        #endregion

        #region Private fields

        private readonly HttpRequest _request;

        #endregion

        #region Constructor

        public HttpRequestToRequestAdapter(HttpRequest request) => _request = request;

        #endregion
    }
}

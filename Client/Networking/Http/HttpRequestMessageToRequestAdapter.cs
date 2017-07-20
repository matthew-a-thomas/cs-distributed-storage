namespace Client.Networking.Http
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using DistributedStorage.Networking.Http;

    /// <summary>
    /// Adapts an <see cref="HttpRequestMessage"/> to an <see cref="IRequest"/>
    /// </summary>
    public sealed class HttpRequestMessageToRequestAdapter : IRequest
    {
        #region Public properties

        public IReadOnlyList<byte> Body
        {
            get
            {
                var task = _requestMessage.Content.ReadAsByteArrayAsync();
                task.Wait();
                return task.Result;
            }
        }

        public string ContentType => _requestMessage.Content.Headers.ContentType.Parameters.FirstOrDefault()?.Value;
        public string Host => _requestMessage.Headers.Host;
        public string Method => _requestMessage.Method.Method;
        public string PathAndQuery => _requestMessage.RequestUri.PathAndQuery;

        #endregion

        #region Private fields

        private readonly HttpRequestMessage _requestMessage;

        #endregion

        #region Constructor

        public HttpRequestMessageToRequestAdapter(HttpRequestMessage requestMessage)
        {
            _requestMessage = requestMessage;
        }

        #endregion
    }
}

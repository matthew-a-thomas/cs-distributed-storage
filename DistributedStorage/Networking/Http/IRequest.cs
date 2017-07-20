namespace DistributedStorage.Networking.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// A web request
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// The raw contents of the request's body
        /// </summary>
        IReadOnlyList<byte> Body { get; }

        /// <summary>
        /// The content type of the request
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// The host for the request
        /// </summary>
        string Host { get; }

        /// <summary>
        /// The request method
        /// </summary>
        string Method { get; }

        /// <summary>
        /// The request path and query string
        /// </summary>
        string PathAndQuery { get; }
    }
}

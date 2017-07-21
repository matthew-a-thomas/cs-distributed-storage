namespace DistributedStorage.Networking.Http.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public sealed class HttpException : Exception
    {
        private static readonly IReadOnlyDictionary<HttpStatusCode, string> CommonExceptions = new Dictionary<HttpStatusCode, string>
        {
            { HttpStatusCode.Unauthorized, "You lack the authorization to perform this action" },
            { HttpStatusCode.Forbidden, "This action is not allowed right now" },
            { HttpStatusCode.NotFound, "The resource was not found" },
            { HttpStatusCode.Conflict, "This action cannot be performed due to the current state of the resource" }
        };

        public static HttpException GenerateException(HttpStatusCode statusCode) => CommonExceptions.TryGetValue(statusCode, out var message) ? new HttpException(statusCode, message) : new HttpException(statusCode, $"An HTTP exception with status code {(int)statusCode} ({statusCode}) has happened.");

        public HttpStatusCode StatusCode { get; }
        
        private HttpException(HttpStatusCode httpStatusCode, string meaning) : base(meaning) => StatusCode = httpStatusCode;
    }
}

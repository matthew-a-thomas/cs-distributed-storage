namespace DistributedStorage.Networking.Http
{
    using System.Net;

    public static class StatusResponse
    {
        public static StatusResponse<T> CreateOk<T>(T value) => new StatusResponse<T>(HttpStatusCode.OK, value);
    }

    public sealed class StatusResponse<T>
    {
        public HttpStatusCode StatusCode { get; }
        public T Value { get; }

        public StatusResponse(HttpStatusCode statusCode, T value)
        {
            StatusCode = statusCode;
            Value = value;
        }
    }
}

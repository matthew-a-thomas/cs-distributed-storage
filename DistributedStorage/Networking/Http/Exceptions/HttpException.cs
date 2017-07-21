namespace DistributedStorage.Networking.Http.Exceptions
{
    using System;

    public abstract class HttpException : Exception
    {
        public int HttpStatusCode { get; }

        protected HttpException(int httpStatusCode, string meaning) : base(meaning) => HttpStatusCode = httpStatusCode;
    }
}

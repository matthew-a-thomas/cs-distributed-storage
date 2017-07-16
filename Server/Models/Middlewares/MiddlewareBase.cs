namespace Server.Models.Middlewares
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public abstract class MiddlewareBase
    {
        protected RequestDelegate Next { get; }

        protected MiddlewareBase(RequestDelegate next)
        {
            Next = next;
        }

        public abstract Task Invoke(HttpContext context);
    }
}

namespace Server.Models.Networking.Http
{
    using System.Net;
    using DistributedStorage.Networking.Http;
    using Microsoft.AspNetCore.Mvc;

    public static class StatusResponseExtensions
    {
        public static IActionResult ToActionResult<T>(this StatusResponse<T> response) => new ObjectResult(response.Value)
        {
            StatusCode = (int)response.StatusCode
        };

        public static IActionResult ToActionResult(this HttpStatusCode statusCode) => new StatusCodeResult((int) statusCode);
    }
}

namespace Server.Models.Networking.Http
{
    using System;
    using System.Threading.Tasks;
    using DistributedStorage.Networking.Http.Exceptions;
    using Microsoft.AspNetCore.Mvc;

    public static class FuncExtensions
    {
        public static async Task<IActionResult> ToActionResultAsync<T>(this Func<Task<T>> asyncFunc)
        {
            try
            {
                var result = await asyncFunc();
                return new OkObjectResult(result);
            }
            catch (HttpException e)
            {
                return new StatusCodeResult((int)e.StatusCode);
            }
        }

        public static async Task<IActionResult> ToActionResultAsync(this Func<Task> asyncFunc)
        {
            try
            {
                await asyncFunc();
                return new OkResult();
            }
            catch (HttpException e)
            {
                return new StatusCodeResult((int)e.StatusCode);
            }
        }
    }
}

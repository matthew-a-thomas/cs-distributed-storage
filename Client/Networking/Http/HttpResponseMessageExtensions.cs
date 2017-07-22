namespace Client.Networking.Http
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Networking.Http;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Asynchronously deserializes the given <paramref name="response"/> into an instance of <typeparamref name="T"/>, or throws an exception
        /// </summary>
        public static async Task<StatusResponse<T>> GetContentsAsStatusResponseAsync<T>(this HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;
            var responseString = await response.Content.ReadAsStringAsync();
            T value;
            if (typeof(string) == typeof(T))
                value = (T)(object)responseString;
            else
                value = JsonConvert.DeserializeObject<T>(responseString);
            return new StatusResponse<T>(statusCode, value);
        }
    }
}

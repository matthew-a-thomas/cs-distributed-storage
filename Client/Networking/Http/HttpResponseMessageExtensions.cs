namespace Client.Networking.Http
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using DistributedStorage.Networking.Http.Exceptions;
    using Newtonsoft.Json;

    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Asserts that <see cref="HttpResponseMessage.IsSuccessStatusCode"/> is true, throwing an <see cref="HttpException"/> if it isn't
        /// </summary>
        public static void AssertSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw HttpException.GenerateException(response.StatusCode);
        }

        /// <summary>
        /// Asynchronously deserializes the given <paramref name="response"/> into an instance of <typeparamref name="T"/>, or throws an exception
        /// </summary>
        public static async Task<T> GetContentsAsync<T>(this HttpResponseMessage response)
        {
            response.AssertSuccess();
            var responseString = await response.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<T>(responseString);
            return deserialized;
        }
    }
}

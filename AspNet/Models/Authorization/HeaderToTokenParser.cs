namespace AspNet.Models.Authorization
{
    using System.Net;
    using Newtonsoft.Json;

    public sealed class HeaderToTokenParser
    {
        public bool TryGetToken(string from, out Token token)
        {
            var json = WebUtility.UrlDecode(from);
            token = JsonConvert.DeserializeObject<Token>(json);
        }
    }
}

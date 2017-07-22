namespace DistributedStorage.Networking.Http
{
    using System;

    public static class UriExtensions
    {
        public static string ToHostHeaderValue(this Uri uri) => $"{uri.Host}{(uri.IsDefaultPort ? "" : $":{uri.Port}")}";
    }
}

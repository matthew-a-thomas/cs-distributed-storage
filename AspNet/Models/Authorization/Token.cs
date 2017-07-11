namespace AspNet.Models.Authorization
{
    using System.Collections.Generic;

    /// <summary>
    /// A certification of certain claims
    /// </summary>
    public sealed class Token
    {
        public Token(IReadOnlyDictionary<string, string> claims, byte[] hmac, byte[] nonce, long unixTimestamp)
        {
            Claims = claims;
            Hmac = hmac;
            Nonce = nonce;
            UnixTimestamp = unixTimestamp;
        }
        
        public IReadOnlyDictionary<string, string> Claims { get; }
        public byte[] Hmac { get; }
        public byte[] Nonce { get; }
        public long UnixTimestamp { get; }
    }
}

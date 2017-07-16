namespace Server.Models.Authorization
{
    public sealed class AuthorizationToken
    {
        public AuthorizationToken(byte[] id, byte[] hmac, byte[] nonce, long unixTime)
        {
            Id = id;
            Hmac = hmac;
            Nonce = nonce;
            UnixTime = unixTime;
        }

        public byte[] Id { get; }
        public byte[] Hmac { get; }
        public byte[] Nonce { get; }
        public long UnixTime { get; }
    }
}

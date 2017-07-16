namespace Server.Models.Authentication
{
    public sealed class Credential
    {
        public Credential(byte[] @public, byte[] @private)
        {
            Public = @public;
            Private = @private;
        }

        public byte[] Public { get; }
        public byte[] Private { get; }
    }
}

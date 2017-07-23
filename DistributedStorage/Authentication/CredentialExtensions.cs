namespace DistributedStorage.Authentication
{
    using Common;
    using System.IO;

    public static class CredentialExtensions
    {
        public static bool TryRead(this Stream stream, out Credential credential)
        {
            credential = null;
            if (!stream.TryRead(out byte[] @private) || !stream.TryRead(out byte[] @public))
                return false;
            credential = new Credential(@public, @private);
            return true;
        }

        public static void Write(this Stream stream, Credential credential)
        {
            stream.Write(credential.Private);
            stream.Write(credential.Public);
        }
    }
}

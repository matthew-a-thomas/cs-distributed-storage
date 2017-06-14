namespace DistributedStorage.Storage
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Networking.Security;

    public static class StorageExtensions
    {
        /// <summary>
        /// Gets or creates our RSA key that is in this given <see cref="Storage"/>.
        /// The given <paramref name="factory"/> is executed when a new key needs to be generated and stored
        /// </summary>
        public static RSAParameters GetOrCreateOurRsaKey(this Storage storage, Func<RSAParameters> factory)
        {
            // First, try reading in our RSA key
            if (storage.OurRsaKeyFile.TryOpenRead(out Stream stream))
                using (stream)
                    if (stream.TryRead(out var key))
                        return key;

            // Otherwise, try opening a stream for writing out our RSA key
            if (!storage.OurRsaKeyFile.TryOpenWrite(out stream))
                throw new Exception("RSA key could neither be read nor written");

            // Write out our RSA key (including the private parameters)
            using (stream)
            {
                var key = factory();
                stream.Write(key, true);
                return key;
            }
        }
    }
}

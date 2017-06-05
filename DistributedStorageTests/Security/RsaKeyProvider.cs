namespace DistributedStorageTests.Security
{
    using System;
    using System.Security.Cryptography;
    using DistributedStorageTests;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides access to some built-in RSA keys useful for testing purposes
    /// </summary>
    internal class RsaKeyProvider
    {
        /// <summary>
        /// An RSA key
        /// </summary>
        internal readonly RSAParameters RsaKey1;

        /// <summary>
        /// An RSA key
        /// </summary>
        internal readonly RSAParameters RsaKey2;

        internal RsaKeyProvider()
        {
            var keys = LoadKeys();
            RsaKey1 = keys[0];
            RsaKey2 = keys[1];
        }

        /// <summary>
        /// Loads RSA keys from <see cref="Resources"/>
        /// </summary>
        private static RSAParameters[] LoadKeys()
        {
            var json = Resources.Sample_RSA_keys;
            if (json == null)
                throw new Exception();
            var keys = JsonConvert.DeserializeObject<RSAParameters[]>(json);
            if (keys == null)
                throw new Exception();
            if (keys.Length != 2)
                throw new Exception();
            return keys;
        }
    }
}

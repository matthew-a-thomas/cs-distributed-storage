namespace DistributedStorage.Networking.Security
{
    using System.Security.Cryptography;
    using Common;
    using Model;

    /// <summary>
    /// An <see cref="IIdentity"/> based on <see cref="RSAParameters"/>
    /// </summary>
    public sealed class RsaIdentity : IIdentity
    {
        /// <summary>
        /// Creates a new <see cref="IIdentity"/> based on the given <paramref name="privateKey"/>
        /// </summary>
        public RsaIdentity(RSAParameters privateKey)
        {
            PrivateKey = privateKey;
            Claim = privateKey.ToHash();
        }
        
        /// <summary>
        /// The <see cref="Hash"/> of this <see cref="RsaIdentity"/>'s public key
        /// </summary>
        public Hash Claim { get; }

        /// <summary>
        /// This <see cref="RsaIdentity"/>'s private key
        /// </summary>
        public RSAParameters PrivateKey { get; }

        /// <summary>
        /// This <see cref="RsaIdentity"/>'s public key
        /// </summary>
        public RSAParameters PublicKey => PrivateKey.ToPublic();
    }
}

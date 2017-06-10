namespace ConsoleApp
{
    using System.Security.Cryptography;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage;

    /// <summary>
    /// Something that manages
    /// an RSA key file,
    /// a folder containing trusted RSA keys,
    /// and a folder containing folders for different <see cref="Manifest"/>s and their associated <see cref="Slice"/>s
    /// </summary>
    public sealed class Storage
    {
        /// <summary>
        /// The folder containing <see cref="Manifest"/>s and their associated <see cref="Slice"/>s
        /// </summary>
        public IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> ContainersForManifests { get; }

        /// <summary>
        /// A file containing an RSA key
        /// </summary>
        public IFile OurRsaKeyFile { get; }

        /// <summary>
        /// The folder containing trusted public RSA keys
        /// </summary>
        public IAddableContainer<Hash, RSAParameters> TrustedPublicKeys { get; }

        /// <summary>
        /// Creates a new <see cref="Storage"/>,
        /// which manages an RSA key file,
        /// a folder containing trusted public RSA keys,
        /// and a folder containing folders for different <see cref="Manifest"/>s and their associated <see cref="Slice"/>s
        /// </summary>
        /// <param name="containersForManifests">The folder containing <see cref="Manifest"/>s and their associated <see cref="Slice"/>s</param>
        /// <param name="ourRsaKeyFile">A file containing an RSA key</param>
        /// <param name="trustedPublicKeys">The folder containing trusted public RSA keys</param>
        public Storage(
            IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>> containersForManifests,
            IFile ourRsaKeyFile,
            IAddableContainer<Hash, RSAParameters> trustedPublicKeys)
        {
            ContainersForManifests = containersForManifests;
            OurRsaKeyFile = ourRsaKeyFile;
            TrustedPublicKeys = trustedPublicKeys;
        }
    }
}

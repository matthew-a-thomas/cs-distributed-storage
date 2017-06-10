namespace ConsoleApp
{
    using DistributedStorage.Storage;

    /// <summary>
    /// Something that can create instances of <see cref="Storage"/>
    /// </summary>
    public sealed class StorageFactory
    {
        /// <summary>
        /// Different options for creating a new <see cref="Storage"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The name of the file containing our RSA key.
            /// This file will be located under the <see cref="PrivateFolderName"/> directory
            /// </summary>
            public string OurRsaKeyName { get; set; } = "ours";

            /// <summary>
            /// The name of the directory that contains private things, such as our RSA key
            /// </summary>
            public string PrivateFolderName { get; set; } = "private";

            /// <summary>
            /// The file extension to use for <see cref="System.Security.Cryptography.RSAParameters"/>
            /// </summary>
            public string KeyExtension { get; set; } = ".rsa";

            /// <summary>
            /// The file extension to use for <see cref="DistributedStorage.Encoding.Manifest"/>s
            /// </summary>
            public string ManifestExtension { get; set; } = ".manifest";

            /// <summary>
            /// The folder that will contain directories for manifests.
            /// Each directory will be named according to the manifest,
            /// will contain a manifest file (named according to the manifest ID, and with an extension of <see cref="ManifestExtension"/>),
            /// and will contain any stored <see cref="DistributedStorage.Encoding.Slice"/>s
            /// </summary>
            public string ManifestsFolderName { get; set; } = "manifests";

            /// <summary>
            /// The file extension to use for stored <see cref="DistributedStorage.Encoding.Slice"/>s
            /// </summary>
            public string SliceExtension { get; set; } = ".slice";

            /// <summary>
            /// The directory in which to store trusted RSA public keys
            /// </summary>
            public string TrustedPublicKeysFolderName { get; set; } = "trusted";
        }
        
        /// <summary>
        /// Creates a new <see cref="Storage"/> using the given <paramref name="options"/>
        /// </summary>
        public Storage CreateStorage(IDirectory workingDirectory, Options options = null)
        {
            options = options ?? new Options();

            // Set up the folders that will contain everything
            var manifestsFolder = workingDirectory.Directories.GetOrCreate(options.ManifestsFolderName);
            var privateFolder = workingDirectory.Directories.GetOrCreate(options.PrivateFolderName);
            var trustedFolder = workingDirectory.Directories.GetOrCreate(options.TrustedPublicKeysFolderName);
            
            // Set up the members that will be injected into the new Storage that's being created
            var manifestsContainer = new ManifestsAndSlicesFactoryContainer(new ManifestsAndSlicesFactoryContainer.Options(options.ManifestExtension, options.SliceExtension, manifestsFolder));
            var ourRsaKeyFile = privateFolder.Files.GetOrCreate($"{options.OurRsaKeyName}{options.KeyExtension}");
            var trustedPublicKeys = new PublicKeysContainer(new PublicKeysContainer.Options(options.KeyExtension, trustedFolder));

            // Now use everything above to create a new Storage object
            return new Storage(manifestsContainer, ourRsaKeyFile, trustedPublicKeys);
        }
    }
}

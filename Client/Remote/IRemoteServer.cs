namespace Client.Remote
{
    using DistributedStorage.Networking.Controllers;

    /// <summary>
    /// A connection to a remote server
    /// </summary>
    public interface IRemoteServer
    {
        /// <summary>
        /// Gets the credential controller
        /// </summary>
        ICredentialController GetCredentialController();

        /// <summary>
        /// Gets the manifests controller
        /// </summary>
        IManifestsController GetManifestsController();

        /// <summary>
        /// Gets the owner controller
        /// </summary>
        IOwnerController GetOwnerController();
    }
}
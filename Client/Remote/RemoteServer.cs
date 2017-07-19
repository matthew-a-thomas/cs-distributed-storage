namespace Client.Remote
{
    using System;
    using System.Net;
    using Controllers;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Controllers;

    public sealed class RemoteServer : IRemoteServer
    {
        #region Public properties

        #endregion

        #region Private fields

        private readonly IPEndPoint _endpoint;
        private readonly Credential _credential;

        #endregion

        #region Constructor

        public RemoteServer(IPEndPoint endpoint, Credential credential)
        {
            _endpoint = endpoint;
            _credential = credential;
        }

        #endregion

        #region Public methods

        public ICredentialController GetCredentialController() => throw new NotImplementedException();

        public IManifestsController GetManifestsController() => new RemoteManifestsController(_credential);

        public IOwnerController GetOwnerController() => throw new NotImplementedException();

        #endregion
    }
}

namespace Client.Remote
{
    using System;
    using System.Net;
    using DistributedStorage.Networking.Controllers;

    public sealed class RemoteServer : IRemoteServer
    {
        #region Public properties

        #endregion

        #region Private fields

        private readonly IPEndPoint _endpoint;

        #endregion

        #region Constructor

        public RemoteServer(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
        }

        #endregion

        #region Public methods

        public ICredentialController GetCredentialController() => throw new NotImplementedException();

        public IManifestsController GetManifestsController() => throw new NotImplementedException();

        public IOwnerController GetOwnerController() => throw new NotImplementedException();

        #endregion
    }
}

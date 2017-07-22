namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using DistributedStorage.Authentication;
    using DistributedStorage.Common;
    using DistributedStorage.Storage.Containers;
    using Remote;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<string, Credential> _credentialContainer;
        private readonly IAddableContainer<string, IRemoteServer> _serverContainer;
        private readonly RemoteServer.Factory _remoteServerFactory;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<string, IRemoteServer> serverContainer,
            RemoteServer.Factory remoteServerFactory,
            IAddableContainer<string, Credential> credentialContainer)
        {
            _serverContainer = serverContainer;
            _remoteServerFactory = remoteServerFactory;
            _credentialContainer = credentialContainer;
        }

        #endregion

        #region Public methods

        public void Run()
        {
            "Pick one".Choose(new Dictionary<string, Action>
            {
                { "List available manifests", ListAvailableManifests },
                { "Manage owned servers", ManageOwnedServers },
                { "Upload file", UploadFile }
            });
        }

        #endregion

        #region Private methods

        private void AddAServer()
        {
            IPAddress ipAddress;
            int port;
            while (!IPAddress.TryParse("IP address?".Ask(), out ipAddress)) { }
            while (!int.TryParse("Port?".Ask(), out port)) { }
            var endpoint = new IPEndPoint(ipAddress, port);
            if (!_credentialContainer.TryGet(endpoint.ToString(), out var credential))
            {
                "No credential could be found for this server, so I'll grab one from this server for you...".Say();
                using (var tempServer = _remoteServerFactory.Create(endpoint, null))
                {
                    var credentialController = tempServer.GetCredentialController();
                    credential = credentialController.GenerateCredentialAsync().WaitAndGet();
                    $"The credential's ID is {Convert.ToBase64String(credential.Public)}".Say();
                }
            }
            var server = _remoteServerFactory.Create(endpoint, credential);
            if (!_serverContainer.TryAdd(server.ToString(), server))
                "Failed to add this server".Say();
        }

        private void DeleteServer(string serverName) => _serverContainer.TryRemove(serverName);

        [SuppressMessage("ReSharper", "UnusedVariable")]
        private void ListAvailableManifests()
        {
            foreach (var kvp in _serverContainer.GetKeysAndValues())
            {
                var serverName = kvp.Key;
                var server = kvp.Value;
            }
        }

        private void ListServers()
        {
            "Pick one".Choose(_serverContainer.GetKeysAndValues().ToDictionary(kvp => kvp.Key, kvp => new Action(() =>
            {
                var serverName = kvp.Key;
                // ReSharper disable once UnusedVariable
                var server = kvp.Value;
                "Do what with this server?".Choose(new Dictionary<string, Action>
                {
                    { "Delete it", () => DeleteServer(serverName) }
                });
            })));
        }

        private void ManageOwnedServers()
        {
            "Do what with servers?".Choose(new Dictionary<string, Action>
            {
                { "List servers", ListServers },
                { "Add a server", AddAServer }
            });
        }

        private void UploadFile() => throw new NotImplementedException();

        #endregion
    }
}

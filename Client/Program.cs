namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using DistributedStorage.Storage.Containers;
    using Remote;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<string, IRemoteServer> _serverContainer;
        private readonly RemoteServer.Factory _remoteServerFactory;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<string, IRemoteServer> serverContainer,
            RemoteServer.Factory remoteServerFactory
            )
        {
            _serverContainer = serverContainer;
            _remoteServerFactory = remoteServerFactory;
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
            var server = _remoteServerFactory.Create(endpoint);
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

namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using DistributedStorage.Storage.Containers;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<string, Server> _serverContainer;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<string, Server> serverContainer
            )
        {
            _serverContainer = serverContainer;
        }

        #endregion

        #region Public methods

        public void Run()
        {
            "Pick one".Choose(new Dictionary<string, Action>
            {
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
            var server = new Server(endpoint);
            if (!_serverContainer.TryAdd(server.ToString(), server))
                "Failed to add this server".Say();
        }

        private void DeleteServer(string serverName) => _serverContainer.TryRemove(serverName);

        private void ListServers()
        {
            "Pick one".Choose(_serverContainer.GetKeysAndValues().ToDictionary(kvp => kvp.Key, kvp => new Action(() =>
            {
                var serverName = kvp.Key;
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

namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using DistributedStorage.Authentication;
    using DistributedStorage.Common;
    using DistributedStorage.Storage.Containers;
    using Remote;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<string, Credential> _credentialContainer;
        private readonly IAddableContainer<Uri, IRemoteServer> _serverContainer;
        private readonly RemoteServer.Factory _remoteServerFactory;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<Uri, IRemoteServer> serverContainer,
            RemoteServer.Factory remoteServerFactory,
            IAddableContainer<string, Credential> credentialContainer)
        {
            _serverContainer = serverContainer;
            _remoteServerFactory = remoteServerFactory;
            _credentialContainer = credentialContainer;
        }

        #endregion

        #region Public methods

        private void Run()
        {
            var shouldLoop = true;
            while (shouldLoop)
            {
                "Pick one".Choose(new Dictionary<string, Action>
                {
                    { "List available manifests", ListAvailableManifests },
                    { "Manage owned servers", ManageOwnedServers },
                    { "Upload file", UploadFile },
                    { "Exit", () => shouldLoop = false }
                });
            }
        }

        #endregion

        #region Private methods

        private void AddAServer()
        {
            Uri baseAddress;
            while (!Uri.TryCreate("Base address?".Ask(), UriKind.Absolute, out baseAddress)) { }
            if (!_credentialContainer.TryGet(baseAddress.ToString(), out var credential))
            {
                "No credential could be found for this server, so I'll grab one from this server for you...".Say();
                using (var tempServer = _remoteServerFactory.Create(baseAddress, null))
                {
                    var credentialController = tempServer.GetCredentialController();
                    credential = credentialController.GenerateCredentialAsync().WaitAndGet();
                    $"The credential's ID is {Convert.ToBase64String(credential.Public)}".Say();
                }
            }
            var server = _remoteServerFactory.Create(baseAddress, credential);
            if (!_serverContainer.TryAdd(baseAddress, server))
                "Failed to add this server".Say();
        }

        private void DeleteServer(Uri serverUri) => _serverContainer.TryRemove(serverUri);

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
            var choices = _serverContainer.GetKeysAndValues().ToDictionary(kvp => kvp.Key.ToString(), kvp => new Action(() =>
            {
                var serverName = kvp.Key;
                // ReSharper disable once UnusedVariable
                var server = kvp.Value;
                "Do what with this server?".Choose(new Dictionary<string, Action>
                {
                    {"Delete it", () => DeleteServer(serverName)}
                });
            }));
            if (choices.Count == 0)
            {
                "You have no saved servers".Say();
                return;
            }
            "Pick one".Choose(choices);
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

namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Authentication;
    using DistributedStorage.Networking.Http.Exceptions;
    using DistributedStorage.Storage.Containers;
    using Remote;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<Uri, Credential> _credentialContainer;
        private readonly IAddableContainer<Uri, IRemoteServer> _serverContainer;
        private readonly RemoteServer.Factory _remoteServerFactory;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<Uri, IRemoteServer> serverContainer,
            RemoteServer.Factory remoteServerFactory,
            IAddableContainer<Uri, Credential> credentialContainer)
        {
            _serverContainer = serverContainer;
            _remoteServerFactory = remoteServerFactory;
            _credentialContainer = credentialContainer;
        }

        #endregion

        #region Public methods

        private async Task RunAsync()
        {
            var shouldLoop = true;
            while (shouldLoop)
            {
                try
                {
                    await "Pick one".ChooseAsync(new Dictionary<string, Func<Task>>
                    {
                        {"List available manifests", ListAvailableManifests},
                        {"Manage owned servers", ManageOwnedServersAsync},
                        {"Upload file", UploadFileAsync},
                        {"Exit", () => Task.Run(() => shouldLoop = false)}
                    });
                }
                catch (Exception e)
                {
                    e.Message.Say();
                }
            }
        }

        #endregion

        #region Private methods

        private async Task AddAServerAsync()
        {
            Uri baseAddress;
            while (!Uri.TryCreate("Base address?".Ask(), UriKind.Absolute, out baseAddress)) { }
            if (_serverContainer.TryGet(baseAddress, out _))
            {
                "This server has already been added. Delete it first".Say();
                return;
            }
            if (!_credentialContainer.TryGet(baseAddress, out var credential))
            {
                "No credential could be found for this server, so I'll grab one from this server for you...".Say();
                await GetAndSaveOwnershipCredentialForAsync(baseAddress);
                if (!_credentialContainer.TryGet(baseAddress, out credential))
                    return;
            }
            var server = _remoteServerFactory.Create(baseAddress, credential);
            if (!_serverContainer.TryAdd(baseAddress, server))
                "Failed to add this server".Say();
        }

        private Task DeleteServerAsync(Uri serverUri) => Task.Run(() => _serverContainer.TryRemove(serverUri));

        private async Task GetAndSaveOwnershipCredentialForAsync(Uri address)
        {
            Credential credential;
            using (var tempServer = _remoteServerFactory.Create(address, null))
            {
                var ownerController = tempServer.GetOwnerController();
                string existingOwner;
                try
                {
                    existingOwner = await ownerController.GetOwnerAsync();
                }
                catch (HttpException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    existingOwner = null;
                }
                if (existingOwner != null)
                {
                    $"This server is already set up for the owner {existingOwner}".Say();
                    return;
                }

                "This server has no existing owner yet. We'll try to make you the owner".Say();
                var credentialController = tempServer.GetCredentialController();
                credential = await credentialController.GenerateCredentialAsync();
                SayPublicKeyOfCredential(credential);
                var tookOwnership = await ownerController.PutOwnerAsync(Convert.ToBase64String(credential.Public));
                if (!tookOwnership)
                {
                    "Failed to make you the owner of this server".Say();
                    return;
                }
            }
            "You are now the owner of this server".Say();
            if (!_credentialContainer.TryAdd(address, credential))
                "Failed to save this credential, even though just a little bit ago there was no credential for this server".Say();
        }
        
        private async Task ListAvailableManifests()
        {
            foreach (var kvp in _serverContainer.GetKeysAndValues())
            {
                var serverName = kvp.Key;
                var server = kvp.Value;

                $"These are the manifests at {serverName}:".Say();
                var manifestsController = server.GetManifestsController();
                foreach (var manifestId in await manifestsController.GetManifestIdsAsync())
                {
                    manifestId.Say();
                }
            }
        }

        private async Task ListServersAsync()
        {
            var choices = _serverContainer.GetKeysAndValues().ToDictionary(kvp => kvp.Key.ToString(), kvp => new Func<Task>(async () =>
            {
                var serverName = kvp.Key;
                var server = kvp.Value;
                await "Do what with this server?".ChooseAsync(new Dictionary<string, Func<Task>>
                {
                    { "Delete it", () => DeleteServerAsync(serverName) },
                    { "View associated credential", () => ViewCredentialForAsync(serverName) }
                });
            }));
            if (choices.Count == 0)
            {
                "You have no saved servers".Say();
                return;
            }
            await "Pick one".ChooseAsync(choices);
        }

        private async Task ManageOwnedServersAsync()
        {
            await "Do what with servers?".ChooseAsync(new Dictionary<string, Func<Task>>
            {
                { "List servers", ListServersAsync },
                { "Add a server", AddAServerAsync }
            });
        }

        private void SayPublicKeyOfCredential(Credential credential) => $"The credential's ID is {Convert.ToBase64String(credential.Public)}".Say();

        private Task UploadFileAsync() => throw new NotImplementedException();

        private async Task ViewCredentialForAsync(Uri serverAddress)
        {
            if (_credentialContainer.TryGet(serverAddress, out var credential))
            {
                SayPublicKeyOfCredential(credential);
            }
            else
            {
                await "You have no credential for this server. Would you like to get one?".ChooseAsync(new Dictionary<string, Func<Task>>
                {
                    {"Yes", () => GetAndSaveOwnershipCredentialForAsync(serverAddress)},
                    {"No", () => Task.CompletedTask }
                });
            }
        }

        #endregion
    }
}

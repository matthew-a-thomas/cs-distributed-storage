namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DistributedStorage.Authentication;
    using DistributedStorage.Storage.Containers;
    using Remote;

    internal partial class Program
    {
        #region Private fields

        private readonly IAddableContainer<Uri, Credential> _credentialsContainerForOwnedServers;
        private readonly RemoteServer.Factory _remoteServerFactory;

        #endregion

        #region Constructor

        public Program(
            IAddableContainer<Uri, Credential> credentialsContainerForOwnedServers,
            RemoteServer.Factory remoteServerFactory)
        {
            _credentialsContainerForOwnedServers = credentialsContainerForOwnedServers;
            _remoteServerFactory = remoteServerFactory;
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
            if (_credentialsContainerForOwnedServers.TryGet(baseAddress, out _))
            {
                "You already own this server.".Say();
                return;
            }
            "You don't yet own this server, so I'll try to get ownership for you...".Say();
            await GetAndSaveOwnershipCredentialForAsync(baseAddress);
            if (!_credentialsContainerForOwnedServers.TryGet(baseAddress, out _))
                "Failed to add this server".Say();
        }

        private Task DeleteCredentialsForOwnedServer(Uri serverUri) => Task.Run(() => _credentialsContainerForOwnedServers.TryRemove(serverUri));

        private async Task GetAndSaveOwnershipCredentialForAsync(Uri address)
        {
            Credential credential;
            using (var tempServer = _remoteServerFactory.Create(address, null))
            {
                var ownerController = tempServer.GetOwnerController();
                var getExistingOwnerResponse = await ownerController.GetOwnerAsync();
                var existingOwner = getExistingOwnerResponse.Value;
                if (getExistingOwnerResponse.StatusCode == HttpStatusCode.OK)
                {
                    $"This server is already set up for the owner {existingOwner}".Say();
                    return;
                }

                "This server has no existing owner yet. We'll try to make you the owner".Say();
                var credentialController = tempServer.GetCredentialController();
                var generateCredentialResponse = await credentialController.GenerateCredentialAsync();
                if (generateCredentialResponse.StatusCode != HttpStatusCode.OK)
                {
                    $"Failed to generate a credential from this server. It returned status code {(int) generateCredentialResponse.StatusCode} ({generateCredentialResponse.StatusCode})".Say();
                    return;
                }
                credential = generateCredentialResponse.Value;
                SayPublicKeyOfCredential(credential);
                var tookOwnershipResponse = await ownerController.PutOwnerAsync(Convert.ToBase64String(credential.Public));
                if (tookOwnershipResponse.StatusCode != HttpStatusCode.OK)
                {
                    $"Failed to take ownership of this server. It returned status code {(int)tookOwnershipResponse.StatusCode} ({tookOwnershipResponse.StatusCode})".Say();
                    return;
                }
                if (!tookOwnershipResponse.Value)
                {
                    "Failed to make you the owner of this server. It returned an OK status code but still failed to make you the owner for some reason".Say();
                    return;
                }
            }
            "You are now the owner of this server".Say();
            if (!_credentialsContainerForOwnedServers.TryAdd(address, credential))
                "Failed to save this credential, even though just a little bit ago there was no credential for this server".Say();
        }
        
        private async Task ListAvailableManifests()
        {
            foreach (var kvp in _credentialsContainerForOwnedServers.GetKeysAndValues())
            {
                var serverUri = kvp.Key;
                var credential = kvp.Value;
                using (var server = _remoteServerFactory.Create(serverUri, credential))
                {
                    $"These are the manifests at {serverUri}:".Say();
                    var manifestsController = server.GetManifestsController();
                    var getManifestIdsResponse = await manifestsController.GetManifestIdsAsync();
                    if (getManifestIdsResponse.StatusCode != HttpStatusCode.OK)
                    {
                        $"Failed to get the manifest IDs from this server. It responded with status code {(int) getManifestIdsResponse.StatusCode} ({getManifestIdsResponse.StatusCode})".Say();
                        continue;
                    }
                    foreach (var manifestId in getManifestIdsResponse.Value)
                    {
                        manifestId.Say();
                    }
                }
            }
        }

        private async Task ListServersAsync()
        {
            var choices = _credentialsContainerForOwnedServers.GetKeysAndValues().ToDictionary(kvp => kvp.Key.ToString(), kvp => new Func<Task>(async () =>
            {
                var serverName = kvp.Key;
                await "Do what with this server?".ChooseAsync(new Dictionary<string, Func<Task>>
                {
                    { "Delete it", () => DeleteCredentialsForOwnedServer(serverName) },
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
            if (_credentialsContainerForOwnedServers.TryGet(serverAddress, out var credential))
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

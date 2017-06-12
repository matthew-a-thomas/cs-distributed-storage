namespace DistributedStorage.Networking
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Actors;
    using Common;
    using Encoding;
    using Security;

    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public sealed class NodeProtocol
    {
        public sealed class Callbacks
        {
            public Func<Manifest[]> GetManifests { get; set; } = () => new Manifest[0];
        }

        private enum Request
        {
            AuthorizedResponse,
            GetManifestTokens
        }

        private readonly Callbacks _callbacks;
        private readonly AuthorizedCommandManager<Stream> _commandManager;
        private readonly ConcurrentDictionary<Request, Action<Stream>> _handlers = new ConcurrentDictionary<Request, Action<Stream>>();
        private readonly SecureStream _otherParty;
        private readonly ITimer _timer;
        private readonly TimeSpan _tokenLifetime;

        public NodeProtocol(Callbacks callbacks, SecureStream otherParty, IEntropy entropy, int tokenSize, ITimer timer, TimeSpan tokenLifetime)
        {
            _callbacks = callbacks;
            _otherParty = otherParty;
            _timer = timer;
            _tokenLifetime = tokenLifetime;
            _commandManager = new AuthorizedCommandManager<Stream>(entropy, tokenSize);

            // Set up handlers
            _handlers.TryAdd(Request.AuthorizedResponse, HandleAuthorizedResponse);
            _handlers.TryAdd(Request.GetManifestTokens, HandleRequestManifestTokens);
        }

        /// <summary>
        /// Handles the other party sending an authorized response
        /// </summary>
        private void HandleAuthorizedResponse(Stream stream)
        {
            // Grab their token
            if (!stream.TryRead(out byte[] token))
                return;

            // Try handling their response
            _commandManager.Invoke(token, stream);
        }

        /// <summary>
        /// Handles the other party requesting our manifest tokens
        /// </summary>
        private void HandleRequestManifestTokens(Stream stream)
        {
            // Grab the token we'll use in responding
            if (!stream.TryRead(out byte[] token))
                return;

            // Get our manifests
            var manifests = _callbacks.GetManifests();

            // Put together our reply datagram
            using (var reply = new MemoryStream())
            {
                // Write the response type
                reply.Write((int) Request.AuthorizedResponse);

                // Write our authorization token
                reply.Write(token);

                // Write how many Manifests are coming
                reply.Write(manifests.Length);

                // Write out each of them
                foreach (var manifest in manifests)
                    reply.Write(manifest);

                // Send our reply
                _otherParty.SendDatagram(reply.ToArray());
            }
        }

        /// <summary>
        /// Pumps our message queue
        /// </summary>
        public void Pump()
        {
            // Wait to receive a datagram from the other party
            if (!_otherParty.TryReceiveDatagram(out var data))
                return;

            using (var stream = new MemoryStream(data))
            {
                // See what kind of request it is
                if (!stream.TryRead(out int requestNumber))
                    return;
                var request = (Request) requestNumber;
                if (!_handlers.TryGetValue(request, out var action))
                    return;

                // Act on the request
                action(stream);
            }
        }

        /// <summary>
        /// Asynchronously requests the other party's list of manifests
        /// </summary>
        public Task<Manifest[]> GetManifestsAsync()
        {
            // Set up a task completion source for the task that we'll work on
            var tcs = new TaskCompletionSource<Manifest[]>();

            // Declare an action that will unschedule the unauthorization of a token
            Action unschedule = null;

            // Set up the action to handle their response
            void Action(Stream stream)
            {
                // Unschedule the unauthorization of the token (if that hasn't already happened)
                Interlocked.Exchange(ref unschedule, null)?.Invoke();
                
                // Figure out how many manifests are coming
                if (!stream.TryRead(out int numManifests))
                    return;

                // Read in all the manifests
                var manifests = new Manifest[numManifests];
                for (var i = 0; i < numManifests; ++i)
                {
                    if (!stream.TryRead(out manifests[i]))
                        return;
                }

                // Now that we have all the manifests, complete the task completion source with them
                tcs.SetResult(manifests);
            }

            // Generate a unique token that will be authorized to execute the action
            byte[] token;
            while (!_commandManager.TryAuthorize(Action, out token))
            { }

            // Now that we have a token, let's schedule its unauthorization for a determinant amount of time in the future, in case we never receive a callback
            unschedule = _timer.Schedule(() =>
                {
                    // Unauthorize the token
                    _commandManager.TryUnauthorize(token);

                    // Cancel the task
                    tcs.TrySetCanceled();
                },
                _tokenLifetime
            );

            // Finally, let the other party know what request we're making, and what their callback token is
            using (var stream = new MemoryStream())
            {
                // Set up our request packet
                stream.Write((int)Request.GetManifestTokens); // Let them know we're requesting their manifests
                stream.Write(token); // Let them know their token for sending the response

                // Send our request packet
                _otherParty.SendDatagram(stream.ToArray());
            }

            // Now that we've done all that, we can return our task completion source
            return tcs.Task;
        }
    }
}

namespace DistributedStorage.Networking.Protocol
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Actors;
    using Common;
    using Security;

    /// <summary>
    /// Facilitates communicating with another party via an <see cref="IDatagramChannel"/>
    /// </summary>
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    [SuppressMessage("ReSharper", "ImplicitlyCapturedClosure")]
    public sealed class DatagramProtocol : IProtocol
    {
        /// <summary>
        /// Creates new <see cref="DatagramProtocol"/>s
        /// </summary>
        public sealed class Factory
        {
            private readonly IEntropy _entropy;
            private readonly ITimer _timer;
            private readonly TimeSpan _tokenLifetime;
            private readonly int _tokenSize;

            /// <summary>
            /// Creates a new <see cref="Factory"/> which creates <see cref="DatagramProtocol"/>s
            /// </summary>
            /// <param name="entropy">The source of entropy for authorization tokens</param>
            /// <param name="timer">Something that can schedule things in the future</param>
            /// <param name="tokenLifetime">The amount of time an authorization token will be valid for</param>
            /// <param name="tokenSize">The number of bytes in an authorization token</param>
            public Factory(IEntropy entropy, ITimer timer, TimeSpan tokenLifetime, int tokenSize)
            {
                _entropy = entropy;
                _timer = timer;
                _tokenLifetime = tokenLifetime;
                _tokenSize = tokenSize;
            }

            /// <summary>
            /// Creates a new <see cref="DatagramProtocol"/> that will talk with the <paramref name="otherParty"/>
            /// </summary>
            public DatagramProtocol Create(IDatagramChannel otherParty) => new DatagramProtocol(otherParty, _entropy, _tokenSize, _timer, _tokenLifetime);
        }

        /// <summary>
        /// Different types of messages that may be sent through this <see cref="DatagramProtocol"/>
        /// </summary>
        private enum MessageType : byte
        {
            /// <summary>
            /// An unsolicited request
            /// </summary>
            Request = 0x00,

            /// <summary>
            /// An authorized response
            /// </summary>
            Response = 0xFF
        }
        
        /// <summary>
        /// Manages authorizations to execute callbacks.
        /// Authorization happens through tokens
        /// </summary>
        private readonly AuthorizedCommandManager<Stream> _commandManager;

        /// <summary>
        /// Things that know how to handle different types of messages
        /// </summary>
        private readonly IReadOnlyDictionary<MessageType, Action<Stream>> _messageHandlers;

        /// <summary>
        /// The other party, with whome we're talking
        /// </summary>
        private readonly IDatagramChannel _otherParty;

        /// <summary>
        /// Things which accept (and handle) unsolicited requests
        /// </summary>
        private readonly ConcurrentDictionary<string, IHandler<byte[], byte[]>> _requestHandlers = new ConcurrentDictionary<string, IHandler<byte[], byte[]>>();

        /// <summary>
        /// Something that knows how to schedule things to happen later
        /// </summary>
        private readonly ITimer _timer;

        /// <summary>
        /// The amount of time an authorization token is considered valid
        /// </summary>
        private readonly TimeSpan _tokenLifetime;

        /// <summary>
        /// Creates a new <see cref="DatagramProtocol"/>, which facilitates communicating with another party via an <see cref="IDatagramChannel"/>
        /// </summary>
        public DatagramProtocol(IDatagramChannel otherParty, IEntropy entropy, int tokenSize, ITimer timer, TimeSpan tokenLifetime)
        {
            _otherParty = otherParty;
            _timer = timer;
            _tokenLifetime = tokenLifetime;
            _commandManager = new AuthorizedCommandManager<Stream>(entropy, tokenSize);

            _messageHandlers = new Dictionary<MessageType, Action<Stream>>
            {
                { MessageType.Request, HandleRequest },
                { MessageType.Response, HandleResponse }
            };
        }

        /// <summary>
        /// Maps an incoming request from the other party to the appropriate <see cref="IHandler{TParameter, TResult}"/> in <see cref="_requestHandlers"/>,
        /// which produces a response which we then send back to the other pary
        /// </summary>
        private void HandleRequest(Stream stream)
        {
            // Pull out the method signature they're asking to invoke
            if (!stream.TryRead(out string signature))
                return;
            // Pull out the parameter
            if (!stream.TryRead(out byte[] parameter))
                return;
            // Pull out the response token
            if (!stream.TryRead(out byte[] token))
                return;
            
            // See if we have a handler for this signature
            if (!_requestHandlers.TryGetValue(signature, out var handler))
                return;

            // Invoke the handler to produce a result to send back
            var response = handler.Handle(parameter);

            using (var responseStream = new MemoryStream())
            {
                // Set up our response datagram
                responseStream.Write((byte)MessageType.Response); // We're making a response
                responseStream.Write(token); // Here's our authorization
                responseStream.Write(response); // Here's our response data

                // Send our response datagram
                _otherParty.SendDatagram(responseStream.ToArray());
            }
        }

        /// <summary>
        /// Handles an authorized response, invoking the appropriate callback command through the <see cref="_commandManager"/>
        /// </summary>
        private void HandleResponse(Stream stream)
        {
            // Pull out their authorization token
            if (!stream.TryRead(out byte[] token))
                return;

            // Try handling their response
            _commandManager.Invoke(token, stream);
        }

        /// <summary>
        /// Asynchronously makes a request to the other party, returning their response when it is ready
        /// </summary>
        public Task<byte[]> MakeRequestAsync(string signature, byte[] parameter)
        {
            var tcs = new TaskCompletionSource<byte[]>();

            // Declare an action that will unschedule the unauthorization of a token
            Action unschedule = null;

            // Set up the action to handle their response
            void Action(Stream stream)
            {
                try
                {
                    // Unschedule the unauthorization of the token (if that hasn't already happened)
                    Interlocked.Exchange(ref unschedule, null)?.Invoke();

                    // Pull out their response
                    if (!stream.TryRead(out byte[] response))
                    {
                        // We couldn't read their response
                        tcs.SetCanceled();
                        return;
                    }
                    
                    // Complete the task with their response
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    // An exception occurred
                    tcs.SetException(e);
                }
            }

            // Generate a unique token that will be authorized to execute the action
#pragma warning disable IDE0018 // Inline variable declaration
            byte[] token;
#pragma warning restore IDE0018 // Inline variable declaration
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

            // Let the other party know our request
            using (var stream = new MemoryStream())
            {
                // Set up our request packet
                stream.Write((byte)MessageType.Request); // We're making a request
                stream.Write(signature); // Here's the signature of the method we're invoking
                stream.Write(parameter); // Here's our parameter
                stream.Write(token); // Here's their token for sending the response

                // Send our request packet
                _otherParty.SendDatagram(stream.ToArray());
            }
            
            // Now that we've done all that, we can return our task completion source
            return tcs.Task;
        }

        /// <summary>
        /// Tries to register the given <paramref name="handler"/> to process requests having the given <paramref name="signature"/>
        /// </summary>
        public bool TryRegister(string signature, IHandler<byte[], byte[]> handler) => _requestHandlers.TryAdd(signature, handler);

        /// <summary>
        /// Tries to unregister the handler associated with the given <paramref name="signature"/> so that messages with this <paramref name="signature"/> will be ignored
        /// </summary>
        public bool TryUnregister(string signature) => _requestHandlers.TryRemove(signature, out _);

        /// <summary>
        /// Pumps our message queue, blocking to process and handle an incoming datagram until one is available
        /// </summary>
        public void Pump()
        {
            // Wait to receive a datagram from the other party
            if (!_otherParty.TryReceiveDatagram(out var data))
                return;

            using (var stream = new MemoryStream(data))
            {
                // See what kind of request it is
                if (!stream.TryRead(out byte messageTypeNumber))
                    return;
                var messageType = (MessageType) messageTypeNumber;

                // Grab a handler for this message type
                if (!_messageHandlers.TryGetValue(messageType, out var action))
                    return;

                // Act on the request
                action(stream);
            }
        }
    }
}

namespace DistributedStorage.Actors
{
    using System;
    using System.Collections.Concurrent;
    using Common;
    using Networking.Security;

    /// <summary>
    /// Something which creates tokens to authorize invocation of commands that accept a parameter
    /// </summary>
    /// <remarks>
    /// Tokens aren't stored directly in memory. Instead, the hashes of tokens are stored in memory.
    /// This guarantees consistent memory requirements.
    /// </remarks>
    public sealed class AuthorizedCommandManager<TParameter>
    {
        /// <summary>
        /// Our source of entropy for creating new tokens
        /// </summary>
        private readonly IEntropy _entropy;

        /// <summary>
        /// The size of a token
        /// </summary>
        private readonly int _tokenSize;

        /// <summary>
        /// Manages the mapping from tokens to authorized commands
        /// </summary>
        private readonly ConcurrentDictionary<Hash, Action<TParameter>> _tokenToCommandMap = new ConcurrentDictionary<Hash, Action<TParameter>>();

        /// <summary>
        /// Creates a new <see cref="AuthorizedCommandManager{TParameter}"/>, which uses tokens to authorize invocation of commands
        /// </summary>
        /// <param name="entropy">The source of entropy for creating new tokens</param>
        /// <param name="tokenSize">The size of created tokens. This doesn't affect the long-term memory requirements of this class, but does affect the likelihood that a token is unique</param>
        public AuthorizedCommandManager(IEntropy entropy, int tokenSize)
        {
            _entropy = entropy;
            _tokenSize = tokenSize;
        }

        /// <summary>
        /// Invokes the command associated with the given <paramref name="token"/>, passing that command the given <paramref name="parameter"/>, if the token is the correct authorization.
        /// Does nothing if the given <paramref name="token"/> isn't authorized.
        /// </summary>
        /// <param name="token">The authorization for invoking a command with the given <paramref name="parameter"/></param>
        /// <param name="parameter">The parameter to pass to the authorized command</param>
        public void Invoke(byte[] token, TParameter parameter)
        {
            var hash = Hash.Create(token);
            if (!_tokenToCommandMap.TryRemove(hash, out var commandForToken))
                return;
            commandForToken(parameter);
        }

        /// <summary>
        /// Creates a new <paramref name="token"/> to authorize invocation of the given <paramref name="command"/>.
        /// Note this creates something in memory that will stick around until either the created <paramref name="token"/> is given to <see cref="TryUnauthorize"/> or until it is used.
        /// Therefore you'll probably want to set a timer to execute <see cref="TryUnauthorize"/> after a determininant period in case the token is never used.
        /// Also note that the probability of this failing is directly related to how large the generated tokens are (according to the tokenSize that was given through the constructor).
        /// </summary>
        public bool TryAuthorize(Action<TParameter> command, out byte[] token)
        {
            token = _entropy.CreateNonce(_tokenSize);
            var hash = Hash.Create(token);
            return _tokenToCommandMap.TryAdd(hash, command);
        }

        /// <summary>
        /// Unauthorizes the given <paramref name="token"/>
        /// </summary>
        public bool TryUnauthorize(byte[] token) => _tokenToCommandMap.TryRemove(Hash.Create(token), out _);
    }
}

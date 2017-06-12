namespace DistributedStorage.Networking.Protocol.Methods
{
    /// <summary>
    /// Creates new <see cref="IMethod{TParameter, TResult}"/>s around <see cref="IProtocol"/>s
    /// </summary>
    public sealed class ProtocolMethodFactory<TParameter, TResult>
    {
        /// <summary>
        /// Serializes and deserializes parameters
        /// </summary>
        private readonly ISerializer<TParameter> _parameterSerializer;

        /// <summary>
        /// Serializes and deserializes method results
        /// </summary>
        private readonly ISerializer<TResult> _resultSerializer;

        /// <summary>
        /// Creates a new <see cref="ProtocolMethodFactory{TParameter,TResult}"/>, which creates new <see cref="IMethod{TParameter, TResult}"/>s around <see cref="IProtocol"/>s
        /// </summary>
        /// <param name="parameterSerializer"></param>
        /// <param name="resultSerializer"></param>
        public ProtocolMethodFactory(ISerializer<TParameter> parameterSerializer, ISerializer<TResult> resultSerializer)
        {
            _parameterSerializer = parameterSerializer;
            _resultSerializer = resultSerializer;
        }

        /// <summary>
        /// Tries to create a new <see cref="IMethod{TParameter, TResult}"/> that will operate on the given <paramref name="protocol"/>
        /// </summary>
        /// <param name="protocol">The protocol through which the returned <paramref name="method"/> will be invoked</param>
        /// <param name="signature">The signature of the <paramref name="method"/> that is being created</param>
        /// <param name="handler">Something that handles requests to the created <paramref name="method"/></param>
        /// <param name="method">The created <see cref="IMethod{TParameter, TResult}"/></param>
        public bool TryCreate(IProtocol protocol, string signature, IHandler<TParameter, TResult> handler, out IMethod<TParameter, TResult> method)
        {
            var serializedHandler = new Handler<byte[], byte[]>(parameterBytes =>
            {
                if (!_parameterSerializer.TryDeserialize(parameterBytes, out var parameter))
                    return null;
                var result = handler.Handle(parameter);
                var resultBytes = _resultSerializer.Serialize(result);
                return resultBytes;
            });
            method = null;
            if (!protocol.TryRegister(signature, serializedHandler))
                return false;
            method = new ProtocolMethod<TParameter, TResult>(
                protocol,
                signature,
                _parameterSerializer,
                _resultSerializer,
                () => protocol.TryUnregister(signature)
            );
            return true;
        }
    }
}

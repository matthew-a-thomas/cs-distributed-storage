namespace DistributedStorage.Networking.Protocol.Methods
{
    using Common;

    /// <summary>
    /// Creates new <see cref="IMethod{TParameter, TResult}"/>s around <see cref="IProtocol"/>s
    /// </summary>
    public sealed class ProtocolMethodFactory<TParameter, TResult> : IProtocolMethodFactory<TParameter, TResult>
    {
        /// <summary>
        /// Converts from a byte array into a <see cref="TParameter"/>
        /// </summary>
        private readonly IConverter<byte[], TParameter> _byteArrayToParameterConverter;

        /// <summary>
        /// Converts from a byte array into a <see cref="TResult"/>
        /// </summary>
        private readonly IConverter<byte[], TResult> _byteArrayToResultConverter;

        /// <summary>
        /// Converts from a <see cref="TParameter"/> to a byte array
        /// </summary>
        private readonly IConverter<TParameter, byte[]> _parameterToByteArrayConverter;

        /// <summary>
        /// Converts from a <see cref="TResult"/> to a byte array
        /// </summary>
        private readonly IConverter<TResult, byte[]> _resultToByteArrayConverter;

        /// <summary>
        /// Creates a new <see cref="ProtocolMethodFactory{TParameter,TResult}"/>, which creates new <see cref="IMethod{TParameter, TResult}"/>s around <see cref="IProtocol"/>s
        /// </summary>
        /// <param name="parameterSerializer"></param>
        /// <param name="resultSerializer"></param>
        public ProtocolMethodFactory(ISerializer<TParameter> parameterSerializer, ISerializer<TResult> resultSerializer)
        {
            (_parameterToByteArrayConverter, _byteArrayToParameterConverter) = parameterSerializer.ToConverters();
            (_resultToByteArrayConverter, _byteArrayToResultConverter) = resultSerializer.ToConverters();
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
            // Create a handler that deals with serialized data
            var serializedHandler = handler.To(_byteArrayToParameterConverter, _resultToByteArrayConverter);

            // Try registering the serialized handler to handle the given signature on the given protocol
            method = null;
            if (!protocol.TryRegister(signature, serializedHandler))
                return false;

            // Create a new method that wraps the protocol, signature, and converters, and that unregisters when it's disposed
            method = new ProtocolMethod<TParameter, TResult>(
                protocol,
                signature,
                _parameterToByteArrayConverter,
                _byteArrayToResultConverter
            );
            return true;
        }
    }
}

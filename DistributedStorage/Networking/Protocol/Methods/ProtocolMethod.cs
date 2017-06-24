namespace DistributedStorage.Networking.Protocol.Methods
{
    using System;
    using Common;

    /// <summary>
    /// An implementation of <see cref="IMethod{TParameter, TResult}"/> that wraps an <see cref="IProtocol"/>
    /// </summary>
    public sealed class ProtocolMethod<TParameter, TResult> : IMethod<TParameter, TResult>
    {
        private readonly IProtocol _protocol;
        private readonly string _signature;
        private readonly IConverter<TParameter, byte[]> _parameterSerializer;
        private readonly IConverter<byte[], TResult> _resultSerializer;

        public ProtocolMethod(IProtocol protocol, string signature, IConverter<TParameter, byte[]> parameterSerializer, IConverter<byte[], TResult> resultSerializer)
        {
            _protocol = protocol;
            _signature = signature;
            _parameterSerializer = parameterSerializer;
            _resultSerializer = resultSerializer;
        }
        
        public void Invoke(TParameter parameter, Action<TResult> callback)
        {
            if (!_parameterSerializer.TryConvert(parameter, out var parameterBytes))
                return;
            _protocol
                .MakeRequestAsync(_signature, parameterBytes)
                .DoAfterSuccess(responseBytes =>
                {
                    if (!_resultSerializer.TryConvert(responseBytes, out var response))
                        return;
                    callback(response);
                });
        }
    }

    public static class ProtocolMethod
    {
        public static ProtocolMethod<TParameter, TResult> Create<TParameter, TResult>(
            IProtocol protocol,
            string signature,
            IConverter<TParameter, byte[]> parameterSerializer,
            IConverter<byte[], TResult> resultSerializer) => new ProtocolMethod<TParameter, TResult>(
                protocol,
                signature,
                parameterSerializer,
                resultSerializer
            );
    }
}

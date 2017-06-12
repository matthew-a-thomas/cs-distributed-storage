namespace DistributedStorage.Networking.Protocol.Methods
{
    using System;
    using System.Threading;

    /// <summary>
    /// An implementation of <see cref="IMethod{TParameter, TResult}"/> that wraps an <see cref="IProtocol"/>
    /// </summary>
    public sealed class ProtocolMethod<TParameter, TResult> : IMethod<TParameter, TResult>
    {
        private readonly IProtocol _protocol;
        private readonly string _signature;
        private readonly ISerializer<TParameter> _parameterSerializer;
        private readonly ISerializer<TResult> _resultSerializer;
        private Action _disposal;

        public ProtocolMethod(IProtocol protocol, string signature, ISerializer<TParameter> parameterSerializer, ISerializer<TResult> resultSerializer, Action disposal)
        {
            _protocol = protocol;
            _signature = signature;
            _parameterSerializer = parameterSerializer;
            _resultSerializer = resultSerializer;
            _disposal = disposal;
        }
        
        public void Invoke(TParameter parameter, Action<TResult> callback)
        {
            var parameterBytes = _parameterSerializer.Serialize(parameter);
            _protocol
                .MakeRequestAsync(_signature, parameterBytes)
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted || task.IsCanceled || task.IsFaulted)
                        return;
                    var responseBytes = task.Result;
                    if (!_resultSerializer.TryDeserialize(responseBytes, out var response))
                        return;
                    callback(response);
                });
        }

        public void Dispose() => Interlocked.Exchange(ref _disposal, null)?.Invoke();
    }
}

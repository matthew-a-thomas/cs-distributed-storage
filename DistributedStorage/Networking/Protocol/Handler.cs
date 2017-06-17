namespace DistributedStorage.Networking.Protocol
{
    using System;

    public sealed class Handler<TParameter, TResult> : IHandler<TParameter, TResult>
    {
        private readonly Func<TParameter, TResult> _handler;

        public Handler(Func<TParameter, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Handle(TParameter parameter) => _handler(parameter);
    }
}

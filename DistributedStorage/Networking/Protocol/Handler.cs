namespace DistributedStorage.Networking.Protocol
{
    using System;
    using Common;

    public sealed class Handler<TParameter, TResult> : IHandler<TParameter, TResult>
    {
        private readonly Func<TParameter, TResult> _handler;

        public Handler(Func<TParameter, TResult> handler)
        {
            _handler = handler;
        }

        public TResult Handle(TParameter parameter) => _handler(parameter);
    }

    public static class Handler
    {
        public static Handler<TParameterIn, TResultOut> CreateFrom<TParameterIn, TParameterIntermediate, TResultIntermediate, TResultOut>(
            IConverter<TParameterIn, TParameterIntermediate> parameterConverter,
            IConverter<TResultIntermediate, TResultOut> resultConverter,
            Func<TParameterIntermediate, TResultIntermediate> intermediateHandler
        ) =>
            new Handler<TParameterIn, TResultOut>(
                parameterIn =>
                parameterConverter.TryConvert(parameterIn, out var parameterIntermediate)
                ? resultConverter.TryConvert(intermediateHandler(parameterIntermediate), out var resultOut)
                ? resultOut
                : default(TResultOut)
                : default(TResultOut)
                );
    }
}

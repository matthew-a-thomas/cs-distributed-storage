namespace DistributedStorage.Networking.Protocol
{
    using Common;

    public static class HandlerExtensions
    {
        /// <summary>
        /// Uses a couple of <see cref="IConverter{TFrom, TTo}"/>s to convert this <see cref="IHandler{TParameter, TResult}"/> into a different one
        /// </summary>
        public static IHandler<TParameterTo, TResultTo> To<TParameterFrom, TResultFrom, TParameterTo, TResultTo>(
            this IHandler<TParameterFrom, TResultFrom> handler,
            IConverter<TParameterTo, TParameterFrom> parameterConverter,
            IConverter<TResultFrom, TResultTo> resultConverter
        ) => new Handler<TParameterTo, TResultTo>(parameter =>
        {
            if (!parameterConverter.TryConvert(parameter, out var convertedParameter))
                return default(TResultTo);
            var result = handler.Handle(convertedParameter);
            resultConverter.TryConvert(result, out var convertedResult);
            return convertedResult;
        });
    }
}

namespace DistributedStorage.Common
{
    /// <summary>
    /// An injected implementation of <see cref="IConverter{TFrom, TTo}"/>
    /// </summary>
    public sealed class Converter<TFrom, TTo> : IConverter<TFrom, TTo>
    {
        /// <summary>
        /// The delegate for <see cref="IConverter{TFrom, TTo}.Convert"/>
        /// </summary>
        public delegate bool TryConvertDelegate(TFrom from, out TTo to);

        /// <summary>
        /// The delegate to use for <see cref="TryConvert"/>
        /// </summary>
        private readonly TryConvertDelegate _tryConvertDelegate;

        /// <summary>
        /// Creates a new <see cref="Converter{TFrom, TTo}"/> using the given <paramref name="tryConvertDelegate"/>
        /// </summary>
        public Converter(TryConvertDelegate tryConvertDelegate)
        {
            _tryConvertDelegate = tryConvertDelegate;
        }

        /// <summary>
        /// Converts from the given thing
        /// </summary>
        public bool TryConvert(TFrom from, out TTo to) => _tryConvertDelegate(from, out to);
    }
}

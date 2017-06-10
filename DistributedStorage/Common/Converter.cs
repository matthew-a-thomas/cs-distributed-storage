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
        public delegate TTo ConvertDelegate(TFrom from);

        /// <summary>
        /// The delegate to use for <see cref="Convert"/>
        /// </summary>
        private readonly ConvertDelegate _convertDelegate;

        /// <summary>
        /// Creates a new <see cref="Converter{TFrom, TTo}"/> using the given <paramref name="convertDelegate"/>
        /// </summary>
        public Converter(ConvertDelegate convertDelegate)
        {
            _convertDelegate = convertDelegate;
        }

        /// <summary>
        /// Converts from the given thing
        /// </summary>
        public TTo Convert(TFrom from) => _convertDelegate(from);
    }
}

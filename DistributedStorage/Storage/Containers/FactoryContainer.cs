namespace DistributedStorage.Storage.Containers
{
    using System.Collections.Generic;

    /// <summary>
    /// An injected implementation of <see cref="IFactoryContainer{TKey, TValue}"/>
    /// </summary>
    public sealed class FactoryContainer<TKey, TValue> : IFactoryContainer<TKey, TValue>
    {
        /// <summary>
        /// Delegate for <see cref="IFactoryContainer{TKey, TValue}.TryCreate"/>
        /// </summary>
        public delegate bool TryCreateDelegate(TKey key, out TValue value);

        /// <summary>
        /// Options for creating a <see cref="FactoryContainer{TKey, TValue}"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The delegate to use for <see cref="FactoryContainer{TKey, TValue}.TryCreate"/>
            /// </summary>
            public TryCreateDelegate TryCreate { get; set; } = DefaultTryCreate;

            /// <summary>
            /// A default implementation that always returns false
            /// </summary>
            private static bool DefaultTryCreate(TKey key, out TValue value)
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// The thing we delegate the <see cref="IAddableContainer{TKey,TValue}"/> portion of <see cref="IFactoryContainer{TKey, TValue}"/> to
        /// </summary>
        private readonly IAddableContainer<TKey, TValue> _container;

        /// <summary>
        /// Injected options needed to complete the <see cref="IFactoryContainer{TKey, TValue}"/> interface
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// Creates a new <see cref="IFactoryContainer{TKey, TValue}"/>, which delegates the <see cref="IAddableContainer{TKey,TValue}"/> part of the interface to the given <paramref name="container"/>, and delegates the rest of the interface to the given <paramref name="options"/>
        /// </summary>
        public FactoryContainer(IAddableContainer<TKey, TValue> container, Options options = null)
        {
            _container = container;
            _options = options ?? new Options();
        }

        public IEnumerable<TKey> GetKeys() => _container.GetKeys();
        
        public bool TryCreate(TKey key, out TValue value) => _options.TryCreate(key, out value);

        public bool TryGet(TKey key, out TValue value) => _container.TryGet(key, out value);

        public bool TryRemove(TKey key) => _container.TryRemove(key);
    }
}

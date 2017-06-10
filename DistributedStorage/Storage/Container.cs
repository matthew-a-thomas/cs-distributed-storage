using System.Collections.Generic;

namespace DistributedStorage.Storage
{
    using System.Linq;

    /// <summary>
    /// A fully-injected implementation of <see cref="IAddableContainer{TKey,TValue}"/>
    /// </summary>
    public sealed class Container<TKey, TValue> : IAddableContainer<TKey, TValue>
    {
        private readonly Options _options;

        public delegate IEnumerable<TKey> GetKeysDelegate();
        public delegate bool TryAddDelegate(TKey key, TValue value);
        public delegate bool TryGetDelegate(TKey key, out TValue value);
        public delegate bool TryRemoveDelegate(TKey key);

        /// <summary>
        /// Various options for creating a <see cref="Container{TKey, TValue}"/>
        /// </summary>
        public sealed class Options
        {
            /// <summary>
            /// The method to use for <see cref="IAddableContainer{TKey,TValue}.GetKeys"/>
            /// </summary>
            public GetKeysDelegate GetKeys { get; set; } = () => Enumerable.Empty<TKey>();

            /// <summary>
            /// The method to use for <see cref="IAddableContainer{TKey,TValue}.TryAdd(TKey, TValue)"/>
            /// </summary>
            public TryAddDelegate TryAdd { get; set; } = (_, __) => false;

            /// <summary>
            /// The method to use for <see cref="IAddableContainer{TKey,TValue}.TryGet(TKey, out TValue)"/>
            /// </summary>
            public TryGetDelegate TryGet { get; set; } = TryGetDefault;

            /// <summary>
            /// The method to use for <see cref="IAddableContainer{TKey,TValue}.TryRemove"/>
            /// </summary>
            public TryRemoveDelegate TryRemove { get; set; } = _ => false;

            /// <summary>
            /// A default implementation of the <see cref="TryGetDelegate"/> delegate
            /// </summary>
            private static bool TryGetDefault(TKey key, out TValue value)
            {
                value = default(TValue);
                return false;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Container{TKey, TValue}"/> using the given <paramref name="options"/>.
        /// If <paramref name="options"/> is null then a Nop/default implementation is used
        /// </summary>
        /// <param name="options"></param>
        public Container(Options options = null)
        {
            _options = options ?? new Options();
        }

        public IEnumerable<TKey> GetKeys() => _options.GetKeys();

        public bool TryAdd(TKey key, TValue value) => _options.TryAdd(key, value);

        public bool TryGet(TKey key, out TValue value) => _options.TryGet(key, out value);

        public bool TryRemove(TKey key) => _options.TryRemove(key);
    }
}

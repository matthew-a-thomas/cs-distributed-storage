namespace DistributedStorage.Common
{
    using System;
    using System.Threading;

    /// <summary>
    /// Something that wraps an instance of <typeparamref name="T"/> in an <see cref="IDisposable"/>
    /// </summary>
    public sealed class DisposableWrapper<T> : IDisposable
    {
        /// <summary>
        /// Gets the wrapped value, or throws an <see cref="ObjectDisposedException"/> if this <see cref="DisposableWrapper{T}"/> has been disposed
        /// </summary>
        public T Value => _dispose != null ? _value : throw new ObjectDisposedException(nameof(DisposableWrapper<T>));

        private Action _dispose;
        private T _value;

        /// <summary>
        /// Creates a new <see cref="DisposableWrapper{T}"/>, which wraps an instance of <typeparamref name="T"/> in an <see cref="IDisposable"/>
        /// </summary>
        public DisposableWrapper(T value, Action dispose)
        {
            _value = value;
            _dispose = dispose;
        }

        /// <summary>
        /// Disposes of this <see cref="DisposableWrapper{T}"/>, which sets the <see cref="Value"/> to default(T) and releases references to the dispose <see cref="Action"/> that was given through the constructor
        /// </summary>
        public void Dispose()
        {
            var dispose = Interlocked.Exchange(ref _dispose, null);
            if (dispose == null)
                return;
            try
            {
                _value = default(T);
            }
            finally
            {
                dispose();
            }
        }
    }
}

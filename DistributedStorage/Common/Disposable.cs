namespace DistributedStorage.Common
{
    using System;
    using System.Threading;

    public sealed class Disposable : IDisposable
    {
        private Action _disposal;

        public Disposable(Action disposal) => _disposal = disposal;

        public void Dispose() => Interlocked.Exchange(ref _disposal, null)?.Invoke();
    }
}

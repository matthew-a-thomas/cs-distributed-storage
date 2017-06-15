namespace DistributedStorage.Actors
{
    using System;
    using System.Threading;

    public static class DispatcherExtensions
    {
        /// <summary>
        /// Invokes the given <paramref name="action"/> on this <see cref="IDispatcher"/>, blocking this thread until it is complete
        /// </summary>
        public static void Invoke(this IDispatcher dispatcher, Action action)
        {
            var gate = new ManualResetEventSlim();
            dispatcher.BeginInvoke(() =>
            {
                try
                {
                    action();
                }
                finally
                {
                    gate.Set();
                }
            });
            gate.Wait();
        }
    }
}

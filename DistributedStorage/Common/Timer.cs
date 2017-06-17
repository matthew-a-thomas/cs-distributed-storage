namespace DistributedStorage.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="ITimer"/> that schedules things to be executed in the <see cref="Task"/> pool
    /// </summary>
    public sealed class Timer : ITimer
    {
        public Action Schedule(Action action, TimeSpan delay)
        {
            var cancellation = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await Task.Delay(delay, cancellation.Token);
                action();
            }, cancellation.Token);
            return cancellation.Cancel;
        }
    }
}

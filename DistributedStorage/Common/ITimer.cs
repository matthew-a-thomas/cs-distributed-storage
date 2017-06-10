namespace DistributedStorage.Common
{
    using System;

    /// <summary>
    /// Something that can schedule things to happen in the future
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// Schedules the given <paramref name="action"/> to be executed after the given <paramref name="delay"/>.
        /// Returns an <see cref="Action"/> that, when invoked, will unschedule this <paramref name="action"/>.
        /// </summary>
        Action Schedule(Action action, TimeSpan delay);
    }
}

namespace DistributedStorage.Actors
{
    using System;

    /// <summary>
    /// Something that allows <see cref="Action"/>s to be scheduled for invocation in a sequential manner
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Schedules this <see cref="Action"/> to be invoked after all previously-scheduled <see cref="Action"/>s are finished
        /// </summary>
        void BeginInvoke(Action action);
    }
}
namespace DistributedStorage.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        /// <summary>
        /// Blocks until this <see cref="Task{T}"/> is done, then returns its <see cref="Task{T}.Result"/>
        /// </summary>
        public static T WaitAndGet<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Blocks until this <see cref="Task{T}"/> is done or until the given <see cref="CancellationToken"/> is cancelled.
        /// Returns true if the task completed successfully
        /// </summary>
        public static bool TryWaitAndGet<T>(this Task<T> task, CancellationToken cancellationToken, out T value)
        {
            value = default(T);
            try
            {
                task.Wait(cancellationToken);
                if (!task.IsSuccessfullyCompleted())
                    return false;
                value = task.Result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if thie <see cref="Task"/> is completed successfully
        /// </summary>
        public static bool IsSuccessfullyCompleted(this Task task) => task.IsCompleted && !task.IsCanceled && !task.IsFaulted;

        /// <summary>
        /// Schedules the given <see cref="Action"/> to execute when this <see cref="Task"/> is completed successfully
        /// </summary>
        public static void DoAfterSuccess(this Task task, Action success) => task.ContinueWith(t =>
        {
            if (!t.IsSuccessfullyCompleted())
                return;
            success();
        });

        /// <summary>
        /// Schedules the given <see cref="Action"/> to execute when this <see cref="Task"/> is completed successfully
        /// </summary>
        public static void DoAfterSuccess<T>(this Task<T> task, Action<T> success) => task.ContinueWith(t =>
        {
            if (!t.IsSuccessfullyCompleted())
                return;
            success(t.Result);
        });
    }
}

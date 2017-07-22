namespace DistributedStorage.Common
{
    using System;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
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

        public static T WaitAndGet<T>(this Task<T> task)
        {
            task.Wait();
            if (!task.IsSuccessfullyCompleted())
                throw task.Exception;
            return task.Result;
        }
    }
}

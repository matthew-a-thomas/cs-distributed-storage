namespace DistributedStorage.Actors
{
    using System;
    using System.Threading;

    /// <summary>
    /// Something which can schedule actions to be invoked later
    /// </summary>
    public sealed class Dispatcher : IDispatcher
    {
        private readonly WorkQueue<Action> _pendingActions;

        internal Dispatcher(WorkQueue<Action> pendingActions)
        {
            _pendingActions = pendingActions;
        }

        /// <summary>
        /// Schedules the given <paramref name="action"/> to be invoked later
        /// </summary>
        public void BeginInvoke(Action action) => _pendingActions.Enqueue(action);

        /// <summary>
        /// Creates a new <see cref="IDispatcher"/>, which executes actions on the given <paramref name="runner"/>
        /// </summary>
        public static IDispatcher Create(Action<Action> runner)
        {
            WorkQueue<Action>.Node workingNode = null;

            void Callback(WorkQueue<Action>.Node node)
            {
                // Set the workingNode if it is null
                if (Interlocked.CompareExchange(ref workingNode, node, null) != null)
                    return; // If workingNode is not null, then the below code is being executed on the runner

                // If we made it to here, then workingNode was null but has been replaced with something non-null.
                // So let's begin running actions that need to be run
                runner(() =>
                {
                    while (true)
                    {
                        workingNode.Value();
                        var next = workingNode = workingNode.Next;
                        if (next == null)
                            break; // workingNode is null, so now it's up to the workQueue to invoke this Callback again for us to start processing actions again
                    }
                });
            }

            var workQueue = new WorkQueue<Action>(Callback);

            var dispatcher = new Dispatcher(workQueue);

            return dispatcher;
        }
    }
}

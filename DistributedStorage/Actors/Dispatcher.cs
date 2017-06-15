namespace DistributedStorage.Actors
{
    using System;
    using System.Threading;

    public sealed class Dispatcher : IDispatcher
    {
        private readonly WorkQueue<Action> _pendingActions;

        internal Dispatcher(WorkQueue<Action> pendingActions)
        {
            _pendingActions = pendingActions;
        }

        public void BeginInvoke(Action action) => _pendingActions.Enqueue(action);

        /// <summary>
        /// Creates a new <see cref="IDispatcher"/>, which executes actions on the given <paramref name="runner"/>
        /// </summary>
        public static IDispatcher Create(Action<Action> runner)
        {
            WorkQueue<Action>.Node workingNode = null;

            void Callback(WorkQueue<Action>.Node node)
            {
                if (Interlocked.CompareExchange(ref workingNode, node, null) != null)
                    return;

                runner(() =>
                {
                    while (true)
                    {
                        workingNode.Value();
                        var next = workingNode = workingNode.Next;
                        if (next == null)
                            break;
                    }
                });
            }

            var workQueue = new WorkQueue<Action>(Callback);

            var dispatcher = new Dispatcher(workQueue);

            return dispatcher;
        }
    }
}

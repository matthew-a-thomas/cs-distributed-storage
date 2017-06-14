namespace DistributedStorage.Actors
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Dispatcher : IDispatcher
    {
        private readonly WorkQueue<Action> _pendingActions;

        internal Dispatcher(WorkQueue<Action> pendingActions)
        {
            _pendingActions = pendingActions;
        }

        public void BeginInvoke(Action action) => _pendingActions.Enqueue(action);

        public static IDispatcher Create()
        {
            WorkQueue<Action>.Node workingNode = null;

            void Callback(WorkQueue<Action>.Node node)
            {
                if (Interlocked.CompareExchange(ref workingNode, node, null) != null)
                    return;

                Task.Run(() =>
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

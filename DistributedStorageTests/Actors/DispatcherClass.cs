namespace DistributedStorageTests.Actors
{
    using System;
    using System.Collections.Concurrent;
    using DistributedStorage.Actors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DispatcherClass
    {
        [TestClass]
        public class BeginInvokeMethod
        {
            [TestMethod]
            public void CausesActionToBeRun()
            {
                Action actionToRun = null;
                var dispatcher = Dispatcher.Create(action => actionToRun = action);
                dispatcher.BeginInvoke(() => { });
                Assert.IsNotNull(actionToRun);
            }

            [TestMethod]
            public void ContinuesToWorkEvenWhenExceptionsAreThrown()
            {
                // Setup
                var actionsToRun = new ConcurrentQueue<Action>();
                var dispatcher = Dispatcher.Create(action => actionsToRun.Enqueue(action));

                // Enqueue a couple of work items
                dispatcher.BeginInvoke(() => throw new Exception());
                var invoked = false;
                dispatcher.BeginInvoke(() => invoked = true);

                // Let the runner run
                while (actionsToRun.TryDequeue(out var actionToRun))
                    actionToRun();

                // Assertions
                Assert.IsTrue(invoked);
            }

            [TestMethod]
            public void DoesNotGetLostWhenRunnerIsBehind()
            {
                // Setup
                var actionsToRun = new ConcurrentQueue<Action>();
                var dispatcher = Dispatcher.Create(action => actionsToRun.Enqueue(action));

                // Enqueue a couple of work items
                dispatcher.BeginInvoke(() => { });
                var invoked = false;
                dispatcher.BeginInvoke(() => invoked = true);

                // Let the runner run
                while (actionsToRun.TryDequeue(out var actionToRun))
                    actionToRun();

                // Assertions
                Assert.IsTrue(invoked);
            }

            [TestMethod]
            public void InvokesOnGivenRunner()
            {
                Action actionToRun = null;
                var dispatcher = Dispatcher.Create(action => actionToRun = action);
                var invoked = false;
                dispatcher.BeginInvoke(() => invoked = true);
                Assert.IsNotNull(actionToRun);
                Assert.IsFalse(invoked);
                actionToRun();
                Assert.IsTrue(invoked);
            }
        }
    }
}

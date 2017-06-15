namespace DistributedStorageTests.Actors
{
    using DistributedStorage.Actors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WorkQueueClass
    {
        [TestClass]
        public class EnqueueMethod
        {
            [TestMethod]
            public void InvokesCallback()
            {
                var invoked = false;
                var queue = new WorkQueue<int>(_ => invoked = true);
                queue.Enqueue(0);
                Assert.IsTrue(invoked);
            }

            [TestMethod]
            public void PassesNodeWithGivenValue()
            {
                var receivedValue = -1;
                var queue = new WorkQueue<int>(node => receivedValue = node.Value);
                queue.Enqueue(5);
                Assert.AreEqual(5, receivedValue);
            }
        }
    }
}

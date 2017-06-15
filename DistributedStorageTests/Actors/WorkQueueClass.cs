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
            public void NodeThatIsPassedToCallbackHasNullNext()
            {
                WorkQueue<int>.Node receivedNode = null;
                var queue = new WorkQueue<int>(node => receivedNode = node);
                queue.Enqueue(0);
                Assert.IsNotNull(receivedNode);
                Assert.AreNotSame(receivedNode, receivedNode.Next);
                Assert.IsNull(receivedNode.Next);
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

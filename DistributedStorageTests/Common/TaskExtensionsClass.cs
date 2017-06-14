namespace DistributedStorageTests.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TaskExtensionsClass
    {
        [TestClass]
        public class DoAfterSuccessMethod
        {
            [TestMethod]
            public void InvokesAfterSuccess1()
            {
                var tcs = new TaskCompletionSource<object>();
                var invoked = false;
                tcs.Task.DoAfterSuccess(() => invoked = true);
                tcs.SetResult(0);
                Thread.Sleep(100);
                Assert.IsTrue(invoked);
            }

            [TestMethod]
            public void InvokesAfterSuccess2()
            {
                var tcs = new TaskCompletionSource<object>();
                var invoked = false;
                tcs.Task.DoAfterSuccess(x => invoked = true);
                tcs.SetResult(0);
                Thread.Sleep(100);
                Assert.IsTrue(invoked);
            }
        }

        [TestClass]
        public class IsSuccessfullyCompletedMethod
        {
            [TestMethod]
            public void ReturnsFalseForCancelledTask()
            {
                var task = Task.FromCanceled(new CancellationToken(true));
                Assert.IsFalse(task.IsSuccessfullyCompleted());
            }

            [TestMethod]
            public void ReturnsFalseForErroredTask()
            {
                var task = Task.FromException(new Exception(""));
                Assert.IsFalse(task.IsSuccessfullyCompleted());
            }

            [TestMethod]
            public void ReturnsTrueForSuccessfullyCompletedTask()
            {
                var task = Task.FromResult(0);
                Assert.IsTrue(task.IsSuccessfullyCompleted());
            }
        }
    }
}

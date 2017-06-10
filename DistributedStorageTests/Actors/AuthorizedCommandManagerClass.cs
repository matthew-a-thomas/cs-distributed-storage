namespace DistributedStorageTests.Actors
{
    using DistributedStorage.Actors;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AuthorizedCommandManagerClass
    {
        private static AuthorizedCommandManager<int> Create() => new AuthorizedCommandManager<int>(new Entropy(), 8);

        [TestClass]
        public class InvokeMethod
        {
            [TestMethod]
            public void DoesNotInvokeUnauthorizedCommand()
            {
                var manager = Create();
                var invoked = false;
                manager.TryAuthorize(_ => invoked = true, out _);
                var otherToken = System.Text.Encoding.ASCII.GetBytes("Other token");
                manager.Invoke(otherToken, 0);
                Assert.IsFalse(invoked);
            }

            [TestMethod]
            public void InvokesAuthorizedCommand()
            {
                var manager = Create();
                var invoked = false;
                Assert.IsTrue(manager.TryAuthorize(_ => invoked = true, out var token));
                manager.Invoke(token, 0);
                Assert.IsTrue(invoked);
            }

            [TestMethod]
            public void SendsAlongParameter()
            {
                var manager = Create();
                var parameter = -1;
                Assert.IsTrue(manager.TryAuthorize(x => parameter = x, out var token));
                manager.Invoke(token, 0);
                Assert.AreEqual(0, parameter);
            }
        }
    }
}

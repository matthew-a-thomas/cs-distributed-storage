namespace DistributedStorageTests.Actors
{
    using DistributedStorage.Actors;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AuthorizedCommandManagerClass
    {
        private static AuthorizedCommandManager<int> Create() => new AuthorizedCommandManager<int>(new CryptoEntropy(), 8);

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

        [TestClass]
        public class TryAuthorizeMethod
        {
            [TestMethod]
            public void ReturnsFalseForDuplicatedTokenThatHasNotBeenUsed()
            {
                var manager = new AuthorizedCommandManager<int>(Helpers.CreateNonsecureEntropy(), 1);
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out _));
                Assert.IsFalse(manager.TryAuthorize(_ => { }, out _));
            }

            [TestMethod]
            public void ReturnsTrueForDuplicatedTokenThatHasBeenUsed()
            {
                var manager = new AuthorizedCommandManager<int>(Helpers.CreateNonsecureEntropy(), 1);
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out var token));
                manager.Invoke(token, 0);
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out _));
            }

            [TestMethod]
            public void ReturnsTrueForUniqueToken()
            {
                var manager = Create();
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out _));
            }
        }

        [TestClass]
        public class TryUnauthorizeMethod
        {
            [TestMethod]
            public void MakesTryAuthorizeReturnTrueForSameToken()
            {
                var manager = new AuthorizedCommandManager<int>(Helpers.CreateNonsecureEntropy(), 1);
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out var token));
                Assert.IsTrue(manager.TryUnauthorize(token));
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out _));
            }

            [TestMethod]
            public void ReturnsFalseWhenNoAuthorizations()
            {
                var manager = Create();
                Assert.IsFalse(manager.TryUnauthorize(new byte[0]));
            }

            [TestMethod]
            public void ReturnsTrueForAuthorization()
            {
                var manager = Create();
                Assert.IsTrue(manager.TryAuthorize(_ => { }, out var token));
                Assert.IsTrue(manager.TryUnauthorize(token));
            }

            [TestMethod]
            public void StopsActionFromBeingInvoked()
            {
                var manager = Create();
                var invoked = false;
                Assert.IsTrue(manager.TryAuthorize(_ => invoked = true, out var token));
                Assert.IsTrue(manager.TryUnauthorize(token));
                manager.Invoke(token, 0);
                Assert.IsFalse(invoked);
            }
        }
    }
}

namespace DistributedStorageTests.Storage
{
    using System.Linq;
    using DistributedStorage.Networking.Security;
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Networking.Security;

    [TestClass]
    public class PublicKeysContainerClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();

        private static PublicKeysContainer Create() => new PublicKeysContainer(new PublicKeysContainer.Options(".rsa", Helpers.CreateDirectory()));

        [TestClass]
        public class TryGetKeysMethod
        {
            [TestMethod]
            public void ReturnsNothingForNewInstance()
            {
                var container = Create();
                Assert.AreEqual(0, container.GetKeys().Count());
            }

            [TestMethod]
            public void ReturnsSomethingAfterOneIsAdded()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
                Assert.AreEqual(1, container.GetKeys().Count());
            }
        }
    }
}

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

        [TestClass]
        public class TryAddMethod
        {
            [TestMethod]
            public void DoesNotAddPrivateKey()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
                Assert.IsTrue(container.TryGet(KeyProvider.RsaKey1.ToHash(), out var key));
                Assert.IsNull(key.D);
                Assert.IsNull(key.DP);
                Assert.IsNull(key.DQ);
                Assert.IsNull(key.InverseQ);
                Assert.IsNull(key.P);
                Assert.IsNull(key.Q);
            }

            [TestMethod]
            public void ReturnsFalseForExistentThing()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
                Assert.IsFalse(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
            }

            [TestMethod]
            public void ReturnsTrueForNonexistentThing()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
            }
        }

        [TestClass]
        public class TryGetMethod
        {
            [TestMethod]
            public void ReturnsFalseForNonexistentThing()
            {
                var container = Create();
                Assert.IsFalse(container.TryGet(KeyProvider.RsaKey1.ToHash(), out _));
            }

            [TestMethod]
            public void ReturnsPreviouslyAddedThing()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
                Assert.IsTrue(container.TryGet(KeyProvider.RsaKey1.ToHash(), out var key));
                Assert.IsTrue(key.ToBytes(false).SequenceEqual(KeyProvider.RsaKey1.ToBytes(false)));
            }
        }

        [TestClass]
        public class TryRemoveMethod
        {
            [TestMethod]
            public void RemovesPreviouslyAddedThing()
            {
                var container = Create();
                Assert.IsTrue(container.TryAdd(KeyProvider.RsaKey1.ToHash(), KeyProvider.RsaKey1));
                Assert.IsTrue(container.TryRemove(KeyProvider.RsaKey1.ToHash()));
                Assert.IsFalse(container.TryGet(KeyProvider.RsaKey1.ToHash(), out _));
            }

            [TestMethod]
            public void ReturnsFalseForNonexistentThing()
            {
                var container = Create();
                Assert.IsFalse(container.TryRemove(KeyProvider.RsaKey1.ToHash()));
            }
        }
    }
}

namespace DistributedStorageTests.Storage
{
    using System.Linq;
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PublicKeysContainerClass
    {
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
        }
    }
}

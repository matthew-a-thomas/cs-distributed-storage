namespace DistributedStorageTests.Storage
{
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ManifestsAndSlicesFactoryContainerClass
    {
        private static ManifestsAndSlicesFactoryContainer Create()
        {
            return new ManifestsAndSlicesFactoryContainer(new ManifestsAndSlicesFactoryContainer.Options(".manifest", ".slice", ))
        }

        [TestClass]
        public class GetKeysMethod
        {
            [TestMethod]
            public void ReturnsNothingForNewInstance()
            {
                
            }
        }
    }
}

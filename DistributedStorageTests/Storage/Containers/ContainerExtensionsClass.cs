namespace DistributedStorageTests.Storage.Containers
{
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Storage.Containers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContainerExtensionsClass
    {
        [TestClass]
        public class ToContainerMethod
        {
            [TestClass]
            public class ReturnValue
            {
                [TestClass]
                public class GetKeysMethod
                {
                    [TestMethod]
                    public void ReturnsAllKeys()
                    {
                        var dictionary = new Dictionary<string, string>();
                        dictionary["1"] = "1";
                        dictionary["2"] = "2";

                        var container = dictionary.ToContainer();
                        Assert.IsTrue(container.GetKeys().SequenceEqual(dictionary.Keys));
                    }
                }
            }
        }
    }
}

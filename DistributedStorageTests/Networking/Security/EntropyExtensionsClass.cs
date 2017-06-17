namespace DistributedStorageTests.Networking.Security
{
    using System;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntropyExtensionsClass
    {
        private static IEntropy CreateIntegerOnlyEntropy(int integer) => new Entropy(size => size == 4 ? BitConverter.GetBytes(integer) : throw new Exception($"{size} bytes were requested, but this is only set up to handle four-byte requests"));

        [TestClass]
        public class NextIntegerMethod
        {
            [TestMethod]
            public void PullsFourBytesFromEntropy()
            {
                var entropy = CreateIntegerOnlyEntropy(0);
                entropy.NextInteger();
            }

            [TestMethod]
            public void ReturnsNumberWithinBoundsWhenGeneratedNumberIsTooLarge()
            {
                var entropy = CreateIntegerOnlyEntropy(4);
                var integer = entropy.NextInteger(1, 4);
                Assert.AreEqual(2, integer);
            }
        }
    }
}

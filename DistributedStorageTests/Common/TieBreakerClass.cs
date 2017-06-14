namespace DistributedStorageTests.Common
{
    using DistributedStorage.Common;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Networking.Security.Utils;

    [TestClass]
    public class TieBreakerClass
    {
        [TestClass]
        public class TestMethod
        {
            [TestMethod]
            public void TheyWinWhenTheirNumberIsLarger()
            {
                var entropy = new NonsecureEntropy();
                var theirValue = entropy.NextInteger() + 1;
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.TheyWon, tieBreaker.Test());
            }

            [TestMethod]
            public void ReturnsTieWhenBothPartiesGenerateSameNumber()
            {
                var entropy = new NonsecureEntropy();
                var theirValue = entropy.NextInteger();
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.Tie, tieBreaker.Test());
            }

            [TestMethod]
            public void YouWinWhenYourNumberIsLarger()
            {
                var entropy = new NonsecureEntropy();
                var theirValue = entropy.NextInteger() - 1;
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.YouWon, tieBreaker.Test());
            }
        }
    }
}

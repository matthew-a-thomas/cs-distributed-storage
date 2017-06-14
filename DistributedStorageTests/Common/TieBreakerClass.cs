namespace DistributedStorageTests.Common
{
    using DistributedStorage.Common;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TieBreakerClass
    {
        [TestClass]
        public class TestMethod
        {
            [TestMethod]
            public void TheyWinWhenTheirNumberIsLarger()
            {
                var entropy = Helpers.CreateNonsecureEntropy();
                var theirValue = entropy.NextInteger() + 1;
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.TheyWon, tieBreaker.Test());
            }

            [TestMethod]
            public void ReturnsTieWhenBothPartiesGenerateSameNumber()
            {
                var entropy = Helpers.CreateNonsecureEntropy();
                var theirValue = entropy.NextInteger();
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.Tie, tieBreaker.Test());
            }

            [TestMethod]
            public void YouWinWhenYourNumberIsLarger()
            {
                var entropy = Helpers.CreateNonsecureEntropy();
                var theirValue = entropy.NextInteger() - 1;
                var tieBreaker = new TieBreaker(entropy, _ => { }, () => theirValue);
                Assert.AreEqual(TieBreaker.Result.YouWon, tieBreaker.Test());
            }
        }
    }
}

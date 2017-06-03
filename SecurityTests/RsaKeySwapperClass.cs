namespace SecurityTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;
    using Utils;

    [TestClass]
    public class RsaKeySwapperClass
    {
        [TestClass]
        public class TrySwapPublicRsaKeysMethod
        {
            [TestMethod]
            public void SwapsKeys()
            {
                var swapper = new RsaKeySwapper(new NonsecureCryptoRsa(), new NonsecureEntropy());
                throw new NotImplementedException();
            }
        }
    }
}

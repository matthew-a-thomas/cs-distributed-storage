namespace SecurityTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;
    using Utils;

    [TestClass]
    public class RsaKeySwapperClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();

        [TestClass]
        public class SendMethod
        {
            [TestMethod]
            public void DoesNotThrowAnException()
            {
                var swapper = new RsaKeySwapper(new NonsecureCryptoRsa(), new NonsecureEntropy());
                using (var stream = new MemoryStream())
                    swapper.Send(stream, KeyProvider.RsaKey1);
            }
        }

        [TestClass]
        public class TryGetMethod
        {
            [TestMethod]
            public void ReturnsWhatSendSent()
            {
                var swapper = new RsaKeySwapper(new NonsecureCryptoRsa(), new NonsecureEntropy());
                using (var stream = new MemoryStream())
                {
                    swapper.Send(stream, KeyProvider.RsaKey1);
                    stream.Position = 0;
                    if (!swapper.TryGet(stream, TimeSpan.FromSeconds(1), out var theirs))
                        throw new Exception("Failed to get their public key");
                    Assert.IsTrue(theirs.ToBytes().SequenceEqual(KeyProvider.RsaKey1.ToBytes()));
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                var swapper = new RsaKeySwapper(new NonsecureCryptoRsa(), new NonsecureEntropy());
                using (var stream = new MemoryStream())
                    Assert.IsFalse(swapper.TryGet(stream, TimeSpan.FromMilliseconds(50), out _));
            }
        }
    }
}

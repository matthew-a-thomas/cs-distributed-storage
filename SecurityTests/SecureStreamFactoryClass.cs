namespace SecurityTests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;
    using Utils;

    [TestClass]
    public class SecureStreamFactoryClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();
        private static readonly SecureStreamFactory SecureStreamFactory = new SecureStreamFactory(new NonsecureCryptoRsa(), new NonsecureCryptoSymmetric(), new NonsecureEntropy());
        
        [TestClass]
        public class TryAcceptConnectionMethod
        {
            [TestMethod]
            public void DoesNotThrowErrorForTimeout()
            {
                using (var stream = new MemoryStream())
                {
                    SecureStreamFactory.TryAcceptConnection(stream, KeyProvider.RsaKey1, TimeSpan.FromMilliseconds(10), out _, out _);
                }
            }
        }

        [TestClass]
        public class TryMakeConnectionMethod
        {
            [TestMethod]
            public void DoesNotThrowErrorForTimeout()
            {
                using (var stream = new MemoryStream())
                {
                    SecureStreamFactory.TryMakeConnection(stream, KeyProvider.RsaKey2, TimeSpan.FromMilliseconds(10), out _, out _);
                }
            }
        }
    }
}

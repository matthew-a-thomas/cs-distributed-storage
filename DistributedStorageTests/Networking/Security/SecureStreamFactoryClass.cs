namespace DistributedStorageTests.Networking.Security
{
    using System.IO;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Utils;

    [TestClass]
    public class SecureStreamFactoryClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();
        private static readonly SecureStreamFactory SecureStreamFactory = new SecureStreamFactory(new NonsecureCryptoRsa(), new NonsecureCryptoSymmetric(), Helpers.CreateNonsecureEntropy());
        
        [TestClass]
        public class TryAcceptConnectionMethod
        {
            [TestMethod]
            public void DoesNotThrowErrorForTimeout()
            {
                using (var stream = new MemoryStream())
                {
                    SecureStreamFactory.TryAcceptConnection(stream, KeyProvider.RsaKey1, out _, out _);
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
                    SecureStreamFactory.TryMakeConnection(stream, KeyProvider.RsaKey2, out _, out _);
                }
            }
        }
    }
}

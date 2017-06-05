namespace DistributedStorageTests.Security
{
    using System;
    using System.Text;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CryptoSymmetricClass
    {
        [TestClass]
        public class TryVerifyHmacAndDecryptMethod
        {
            [TestMethod]
            public void ReturnsFalseForReplayedCiphertext()
            {
                var crypto = new CryptoSymmetric(TimeSpan.FromSeconds(1));
                var key = crypto.ConvertToAesKey(Encoding.ASCII.GetBytes("This is the super secret key"));
                var ciphertext = crypto.EncryptAndHmac(Encoding.ASCII.GetBytes("Hello world"), key);
                Assert.IsTrue(crypto.TryVerifyHmacAndDecrypt(ciphertext, key, out _));
                Assert.IsFalse(crypto.TryVerifyHmacAndDecrypt(ciphertext, key, out _));
            }
        }
    }
}

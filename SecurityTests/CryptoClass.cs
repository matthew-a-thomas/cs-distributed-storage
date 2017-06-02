namespace SecurityTests
{
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;

    [TestClass]
    public class CryptoClass
    {
        [TestClass]
        public class CreateAesKeyMethod
        {
            [TestMethod]
            public void ReturnsNonEmptyArray()
            {
                var array = Crypto.CreateAesKey();
                Assert.IsNotNull(array);
                Assert.IsTrue(array.Length > 0);
            }
        }

        [TestClass]
        public class DecryptAesMethod
        {
            [TestMethod]
            public void ReturnsNullForPlaintext()
            {
                var key = Crypto.CreateAesKey();
                var result = Crypto.DecryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                Assert.IsNull(result);
            }

            [TestMethod]
            public void DecryptsWhatEncryptReturns()
            {
                var key = Crypto.CreateAesKey();
                var ciphertext = Crypto.EncryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                var plaintext = Crypto.DecryptAes(ciphertext, key);
                Assert.IsNotNull(plaintext);
                var message = Encoding.ASCII.GetString(plaintext);
                Assert.AreEqual("Hello world", message);
            }
        }

        [TestClass]
        public class EncryptAesMethod
        {
            [TestMethod]
            public void ReturnsNonNull()
            {
                var key = Crypto.CreateAesKey();
                var ciphertext = Crypto.EncryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                Assert.IsNotNull(ciphertext);
            }

            [TestMethod]
            public void ReturnsSomethingOtherThanWhatWentIn()
            {
                var key = Crypto.CreateAesKey();
                var ciphertext = Crypto.EncryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                var message = Encoding.ASCII.GetString(ciphertext);
                Assert.AreNotEqual("Hello world", message);
            }
        }
    }
}

namespace SecurityTests
{
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;

    [TestClass]
    public class CryptoClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();

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
            public void DecryptsWhatEncryptReturns()
            {
                var key = Crypto.CreateAesKey();
                var ciphertext = Crypto.EncryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                var plaintext = Crypto.DecryptAes(ciphertext, key);
                Assert.IsNotNull(plaintext);
                var message = Encoding.ASCII.GetString(plaintext);
                Assert.AreEqual("Hello world", message);
            }

            [TestMethod]
            public void ReturnsNullForPlaintext()
            {
                var key = Crypto.CreateAesKey();
                var result = Crypto.DecryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                Assert.IsNull(result);
            }
        }

        [TestClass]
        public class CreateRsaKeyMethod
        {
            [TestMethod]
            public void ReturnsNonDefaultValue()
            {
                Assert.IsTrue(
                    new[] { KeyProvider.RsaKey1.D, KeyProvider.RsaKey1.DP, KeyProvider.RsaKey1.DQ, KeyProvider.RsaKey1.Exponent, KeyProvider.RsaKey1.InverseQ, KeyProvider.RsaKey1.Modulus, KeyProvider.RsaKey1.P, KeyProvider.RsaKey1.Q}.All(
                        x => !ReferenceEquals(null, x) && x.Length > 0));
            }
        }

        [TestClass]
        public class DecryptRsaMethod
        {
            [TestMethod]
            public void DecryptsWhatEncryptReturns()
            {
                var ciphertext = Crypto.EncryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2);
                var plaintext = Crypto.DecryptRsa(ciphertext, KeyProvider.RsaKey2, KeyProvider.RsaKey1);
                Assert.IsNotNull(plaintext);
                var message = Encoding.ASCII.GetString(plaintext);
                Assert.AreEqual("Hello world", message);
            }

            [TestMethod]
            public void ReturnsNullForPlaintext()
            {
                var result = Crypto.DecryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2);
                Assert.IsNull(result);
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

        [TestClass]
        public class EncryptRsaMethod
        {
            [TestMethod]
            public void ReturnsNonNull()
            {
                var ciphertext = Crypto.EncryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2);
                Assert.IsNotNull(ciphertext);
            }

            [TestMethod]
            public void ReturnsSomethingOtherThanWhatWentIn()
            {
                var ciphertext = Crypto.EncryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2);
                var message = Encoding.ASCII.GetString(ciphertext);
                Assert.AreNotEqual("Hello world", message);
            }
        }
    }
}

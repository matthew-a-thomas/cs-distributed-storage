namespace SecurityTests
{
    using System;
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
        public class TryDecryptAesMethod
        {
            [TestMethod]
            public void DecryptsWhatEncryptReturns()
            {
                var key = Crypto.CreateAesKey();
                var ciphertext = Crypto.EncryptAes(Encoding.ASCII.GetBytes("Hello world"), key);
                Assert.IsTrue(Crypto.TryDecryptAes(ciphertext, key, out var plaintext, out var ticksUtc));
                Assert.IsTrue(DateTime.UtcNow.Ticks - ticksUtc < TimeSpan.FromSeconds(1).Ticks);
                Assert.IsNotNull(plaintext);
                var message = Encoding.ASCII.GetString(plaintext);
                Assert.AreEqual("Hello world", message);
            }

            [TestMethod]
            public void ReturnsFalseForPlaintext()
            {
                var key = Crypto.CreateAesKey();
                Assert.IsFalse(Crypto.TryDecryptAes(Encoding.ASCII.GetBytes("Hello world"), key, out _, out _));
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
        public class TryDecryptRsaMethod
        {
            [TestMethod]
            public void DecryptsWhatEncryptReturns()
            {
                var ciphertext = Crypto.EncryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2);
                Assert.IsTrue(Crypto.TryDecryptRsa(ciphertext, KeyProvider.RsaKey2, KeyProvider.RsaKey1, out var plaintext));
                Assert.IsNotNull(plaintext);
                var message = Encoding.ASCII.GetString(plaintext);
                Assert.AreEqual("Hello world", message);
            }

            [TestMethod]
            public void ReturnsFalseForPlaintext()
            {
                Assert.IsFalse(Crypto.TryDecryptRsa(Encoding.ASCII.GetBytes("Hello world"), KeyProvider.RsaKey1, KeyProvider.RsaKey2, out _));
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

namespace DistributedStorageTests.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Utils;

    [TestClass]
    public class SecureStreamClass
    {
        [TestClass]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Constructor
        {
            [TestMethod]
            public void AcceptsSmallConnectionKey()
            {
                using (var stream = new MemoryStream())
                    new SecureStream(stream, new byte[1], new NonsecureCryptoSymmetric());
            }

            [TestMethod]
            public void AcceptsLargeConnectionKey()
            {
                using (var stream = new MemoryStream())
                    new SecureStream(stream, new byte[10000], new NonsecureCryptoSymmetric());
            }
        }

        [TestClass]
        public class SendDatagramMethod
        {
            [TestMethod]
            public void SendsSomethingOtherThanThePlaintext()
            {
                var plaintext = Encoding.ASCII.GetBytes("Hello world");
                using (var stream = new MemoryStream())
                {
                    new SecureStream(stream, new byte[1], new NonsecureCryptoSymmetric()).SendDatagram(plaintext);
                    stream.Position = 0;
                    var written = stream.ToArray();
                    Assert.IsFalse(plaintext.SequenceEqual(written));
                }
            }

            [TestMethod]
            public void WorksWithSmallConnectionKey()
            {
                using (var stream = new MemoryStream())
                    new SecureStream(stream, new byte[1], new NonsecureCryptoSymmetric()).SendDatagram(Encoding.ASCII.GetBytes("Hello world"));
            }

            [TestMethod]
            public void WorksWithLargeConnectionKey()
            {
                using (var stream = new MemoryStream())
                    new SecureStream(stream, new byte[10000], new NonsecureCryptoSymmetric()).SendDatagram(Encoding.ASCII.GetBytes("Hello world"));
            }
        }

        [TestClass]
        public class TryReceiveDatagramMethod
        {
            [TestMethod]
            public void ReceivesWhatSendDatagramSentWithSmallKey()
            {
                using (var stream = new MemoryStream())
                {
                    var key = new byte[1];
                    var message = Encoding.ASCII.GetBytes("Hello world");
                    new SecureStream(stream, key, new NonsecureCryptoSymmetric()).SendDatagram(message);
                    stream.Position = 0;
                    Assert.IsTrue(new SecureStream(stream, key, new NonsecureCryptoSymmetric()).TryReceiveDatagram(TimeSpan.FromMilliseconds(100), out var plaintext));
                    Assert.IsTrue(message.SequenceEqual(plaintext));
                }
            }

            [TestMethod]
            public void ReceivesWhatSendDatagramSentWithLargeKey()
            {
                using (var stream = new MemoryStream())
                {
                    var key = new byte[10000];
                    var message = Encoding.ASCII.GetBytes("Hello world");
                    new SecureStream(stream, key, new NonsecureCryptoSymmetric()).SendDatagram(message);
                    stream.Position = 0;
                    Assert.IsTrue(new SecureStream(stream, key, new NonsecureCryptoSymmetric()).TryReceiveDatagram(TimeSpan.FromMilliseconds(100), out var plaintext));
                    Assert.IsTrue(message.SequenceEqual(plaintext));
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                using (var stream = new MemoryStream())
                    Assert.IsFalse(new SecureStream(stream, new byte[10], new NonsecureCryptoSymmetric()).TryReceiveDatagram(TimeSpan.FromMilliseconds(10), out _));
            }
        }
    }
}

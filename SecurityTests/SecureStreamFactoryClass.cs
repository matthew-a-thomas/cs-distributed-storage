namespace SecurityTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;
    using Utils;

    [TestClass]
    public class SecureStreamFactoryClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();
        private static readonly SecureStreamFactory SecureStreamFactory = new SecureStreamFactory(new NonsecureCryptoRsa(), new NonsecureCryptoSymmetric(), new NonsecureEntropy());

        [TestClass]
        public class ConnectionTests
        {
            [TestMethod]
            public void TryMakingSecureStreamsAndThenSendingSomething()
            {
                (var stream1, var stream2) = Pipe.CreateLinkedPair();
                using (stream1)
                {
                    using (stream2)
                    {
                        SecureStream
                            secureStream1 = null,
                            secureStream2 = null;
                        Task.WaitAll(
                            Task.Run(() =>
                            {
                                if (!SecureStreamFactory.TryMakeConnection(
                                    stream1,
                                    KeyProvider.RsaKey1,
                                    TimeSpan.FromSeconds(10),
                                    out var theirs,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream1)}");
                                Assert.IsTrue(theirs.ToBytes().SequenceEqual(KeyProvider.RsaKey2.ToBytes()));
                                secureStream1 = secureStream;
                            }),
                            Task.Run(() =>
                            {
                                if (!SecureStreamFactory.TryAcceptConnection(
                                    stream2,
                                    KeyProvider.RsaKey2,
                                    TimeSpan.FromSeconds(10),
                                    out var theirs,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream2)}");
                                Assert.IsTrue(theirs.ToBytes().SequenceEqual(KeyProvider.RsaKey1.ToBytes()));
                                secureStream2 = secureStream;
                            })
                        );
                        secureStream1.SendDatagram(Encoding.ASCII.GetBytes("Hello world"));
                        if (!secureStream2.TryReceiveDatagram(TimeSpan.FromSeconds(1), out var data))
                            throw new Exception($"Didn't receive anything on {nameof(secureStream2)}");
                        var message = Encoding.ASCII.GetString(data);
                        Assert.AreEqual("Hello world", message);
                    }
                }
            }
        }

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

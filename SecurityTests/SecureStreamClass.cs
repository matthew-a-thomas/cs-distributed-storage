namespace SecurityTests
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;

    [TestClass]
    public class SecureStreamClass
    {
        private static bool _initialized;
        private static readonly object InitializationLockObject = new object();
        private static RSAParameters
            _rsaKey1,
            _rsaKey2;

        private static void Initialize()
        {
            lock (InitializationLockObject)
            {
                if (_initialized)
                    return;

                Task.WaitAll(
                    Task.Run(() => _rsaKey1 = Crypto.CreateRsaKey()),
                    Task.Run(() => _rsaKey2 = Crypto.CreateRsaKey())
                );

                _initialized = true;
            }
        }
        
        [TestClass]
        public class ConnectionTests
        {
            public ConnectionTests()
            {
                Initialize();
            }

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
                                if (!SecureStream.TryMakeConnection(
                                    stream1,
                                    _rsaKey1,
                                    TimeSpan.FromSeconds(1),
                                    out var theirs,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream1)}");
                                Assert.IsTrue(theirs.ToBytes().SequenceEqual(_rsaKey2.ToBytes()));
                                secureStream1 = secureStream;
                            }),
                            Task.Run(() =>
                            {
                                if (!SecureStream.TryAcceptConnection(
                                    stream2,
                                    _rsaKey2,
                                    TimeSpan.FromSeconds(1),
                                    out var theirs,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream2)}");
                                Assert.IsTrue(theirs.ToBytes().SequenceEqual(_rsaKey1.ToBytes()));
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
    }
}

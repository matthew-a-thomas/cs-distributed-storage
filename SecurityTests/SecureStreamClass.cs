namespace SecurityTests
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Security;

    [TestClass]
    public class SecureStreamClass
    {
        private static readonly RSAParameters
            RsaKey1 = Crypto.CreateRsaKey(),
            RsaKey2 = Crypto.CreateRsaKey();

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
                                if (!SecureStream.TryMakeConnection(
                                    stream1,
                                    RsaKey1,
                                    out _,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream1)}");
                                secureStream1 = secureStream;
                            }),
                            Task.Run(() =>
                            {
                                if (!SecureStream.TryAcceptConnection(
                                    stream2,
                                    RsaKey2,
                                    out _,
                                    out var secureStream
                                ))
                                    throw new Exception($"Couldn't make {nameof(secureStream2)}");
                                secureStream2 = secureStream;
                            })
                        );
                        secureStream1.SendDatagram(Encoding.ASCII.GetBytes("Hello world"));
                        if (!secureStream2.TryReceiveDatagram(out var data))
                            throw new Exception($"Didn't receive anything on {nameof(secureStream2)}");
                        var message = Encoding.ASCII.GetString(data);
                        Assert.AreEqual("Hello world", message);
                    }
                }
            }
        }
    }
}

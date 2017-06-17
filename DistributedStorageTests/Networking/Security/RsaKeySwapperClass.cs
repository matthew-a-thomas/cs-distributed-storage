namespace DistributedStorageTests.Networking.Security
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using DistributedStorage.Networking.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Utils;

    [TestClass]
    public class RsaKeySwapperClass
    {
        private static readonly RsaKeyProvider KeyProvider = new RsaKeyProvider();

        private static RsaKeySwapper CreateKeySwapper() => new RsaKeySwapper(new NonsecureCryptoRsa());

        [TestClass]
        public class SendChallengeMethod
        {
            [TestMethod]
            public void DoesNotThrowAnException()
            {
                var swapper = CreateKeySwapper();
                using (var stream = new MemoryStream())
                    swapper.SendChallenge(stream, KeyProvider.RsaKey1, new byte[0]);
            }
        }

        [TestClass]
        public class TryReceiveChallengeMethod
        {
            [TestMethod]
            public void ReceivesFromSendChallenge()
            {
                var swapper = CreateKeySwapper();
                using (var stream = new MemoryStream())
                {
                    var challenge = Encoding.ASCII.GetBytes("Hello world");
                    swapper.SendChallenge(stream, KeyProvider.RsaKey1, challenge);
                    stream.Position = 0;
                    Assert.IsTrue(swapper.TryReceiveChallenge(stream, out var theirs, out var theirChallenge));
                    Assert.IsTrue(theirs.ToBytes(false).SequenceEqual(KeyProvider.RsaKey1.ToBytes(false)));
                    Assert.IsTrue(theirChallenge.SequenceEqual(challenge));
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                var swapper = CreateKeySwapper();
                using (var stream = new MemoryStream())
                    Assert.IsFalse(swapper.TryReceiveChallenge(stream, out _, out _));
            }
        }

        [TestClass]
        public class SendChallengeResponseMethod
        {
            [TestMethod]
            public void DoesNotThrowAnException()
            {
                using (var stream = new MemoryStream())
                    CreateKeySwapper().SendChallengeResponse(stream, KeyProvider.RsaKey1, new byte[0], new byte[0]);
            }
        }

        [TestClass]
        public class TryReceiveChallengeResponseMethod
        {
            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                using (var stream = new MemoryStream())
                    Assert.IsFalse(CreateKeySwapper().TryReceiveChallengeResponse(stream, new byte[0], new byte[0], KeyProvider.RsaKey2));
            }

            [TestMethod]
            public void ReturnsTrueFromSendChallengeResponse()
            {
                var swapper = CreateKeySwapper();
                using (var stream = new MemoryStream())
                {
                    var challenge1 = Encoding.ASCII.GetBytes("Hello world!");
                    var challenge2 = Encoding.ASCII.GetBytes("HELLO WORLD.");
                    swapper.SendChallengeResponse(stream, KeyProvider.RsaKey1, challenge2, challenge1);
                    stream.Position = 0;
                    Assert.IsTrue(swapper.TryReceiveChallengeResponse(stream, challenge2, challenge1, KeyProvider.RsaKey1));
                }
            }
        }
    }
}

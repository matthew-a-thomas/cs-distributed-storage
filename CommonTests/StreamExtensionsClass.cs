namespace CommonTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Common;

    [TestClass]
    public class StreamExtensionsClass
    {
        [TestClass]
        public class BlockingReadMethod
        {
            [TestMethod]
            public void ReadsCorrectBytes()
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 }))
                {
                    var data = stream.BlockingRead(3);
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }

            [TestMethod]
            public void DoesNotReturnIfBytesAreNotAvailable()
            {
                Assert.IsFalse(Task.Run(() =>
                    {
                        using (var stream = new MemoryStream())
                        {
                            stream.BlockingRead(1);
                        }
                    })
                    .Wait(TimeSpan.FromMilliseconds(500)));
            }
        }

        [TestClass]
        public class ReadChunkMethod
        {
            [TestMethod]
            public void ReadsWhatWriteChunkPutIn()
            {
                using (var stream = new MemoryStream())
                {
                    stream.WriteChunk(new byte[] { 0x01, 0x02, 0x03 });
                    stream.Position = 0;
                    var data = stream.BlockingReadChunk();
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }
        }

        [TestClass]
        public class WriteMethod
        {
            [TestMethod]
            public void PutsCorrectNumberOfBytesInStream()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(new byte[] { 0x01, 0x02, 0x03 });
                    Assert.AreEqual(3, stream.Position);
                }
            }
        }

        [TestClass]
        public class WriteChunkMethod
        {
            [TestMethod]
            public void PutsCorrectBytesInStream()
            {
                using (var stream = new MemoryStream())
                {
                    stream.WriteChunk(new byte[] { 0x01, 0x02, 0x03 });
                    var data = stream.ToArray();
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x03, 0, 0, 0, 0x01, 0x02, 0x03 }));
                }
            }
        }
    }
}

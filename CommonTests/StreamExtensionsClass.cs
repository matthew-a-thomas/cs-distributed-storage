namespace CommonTests
{
    using System;
    using System.IO;
    using System.Linq;
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
                    var data = stream.BlockingRead(3, TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }

            [TestMethod]
            public void DoesNotReturnIfBytesAreNotAvailable()
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.BlockingRead(1, TimeSpan.FromMilliseconds(500));
                    }
                }
                catch (TimeoutException)
                { } // A timeout exception means this test was successful
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
                    var data = stream.BlockingReadChunk(TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }
        }

        [TestClass]
        public class TryReadMethod
        {
            [TestMethod]
            public void ReturnsFalseWhenNotEnoughBytes()
            {
                using (var stream = new MemoryStream())
                {
                    Assert.IsFalse(stream.TryRead(1, out _));
                }
            }
        }

        [TestClass]
        public class TryReadChunkMethod
        {
            [TestMethod]
            public void ReturnsFalseWhenNotEnoughBytes()
            {
                using (var stream = new MemoryStream())
                {
                    Assert.IsFalse(stream.TryReadChunk(out _));
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

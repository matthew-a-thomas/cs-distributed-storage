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
        public class TryBlockingReadMethod
        {
            [TestMethod]
            public void ReadsCorrectBytes()
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 }))
                {
                    Assert.IsTrue(stream.TryBlockingRead(3, TimeSpan.FromSeconds(1), out var data));
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                using (var stream = new MemoryStream())
                    Assert.IsFalse(stream.TryBlockingRead(1, TimeSpan.FromMilliseconds(100), out _));
            }
        }

        [TestClass]
        public class TryBlockingReadChunkMethod
        {
            [TestMethod]
            public void ReadsWhatWriteChunkPutIn()
            {
                using (var stream = new MemoryStream())
                {
                    stream.WriteChunk(new byte[] { 0x01, 0x02, 0x03 });
                    stream.Position = 0;
                    Assert.IsTrue(stream.TryBlockingReadChunk(TimeSpan.FromSeconds(1), out var data));
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenTimingOut()
            {
                using (var stream = new MemoryStream())
                    Assert.IsFalse(stream.TryBlockingReadChunk(TimeSpan.FromMilliseconds(10), out _));
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

            [TestMethod]
            public void ReturnsWhatWriteChunkWrites()
            {
                using (var stream = new MemoryStream())
                {
                    stream.WriteChunk(new byte[] { 0x01, 0x02, 0x03 });
                    stream.Position = 0;
                    Assert.IsTrue(stream.TryReadChunk(out var chunk));
                    Assert.IsTrue(chunk.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
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
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x03, 0x01, 0x02, 0x03 })); // We use Variable-length Quantity to encode how many bytes follow, hence the starting "0x03"
                }
            }
        }
    }
}

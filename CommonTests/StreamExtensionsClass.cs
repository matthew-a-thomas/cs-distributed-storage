namespace CommonTests
{
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Common;

    [TestClass]
    public class StreamExtensionsClass
    {
        [TestClass]
        public class ReadMethod
        {
            [TestMethod]
            public void ReadsCorrectBytes()
            {
                using (var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 }))
                {
                    var data = stream.Read(3);
                    Assert.IsNotNull(data);
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
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
                    var data = stream.ReadChunk();
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

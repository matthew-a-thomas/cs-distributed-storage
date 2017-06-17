namespace DistributedStorageTests.Common
{
    using System.IO;
    using System.Linq;
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StreamExtensionsClass
    {
        [TestClass]
        public class TryReadMethod
        {
            [TestMethod]
            public void ReadsNullThatWasWrittenWithWrite()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write((byte[])null);
                    stream.Position = 0;
                    Assert.IsTrue(stream.TryRead(out byte[] bytes));
                    Assert.IsNull(bytes);
                }
            }

            [TestMethod]
            public void ReturnsFalseWhenNotEnoughBytes()
            {
                using (var stream = new MemoryStream())
                {
                    Assert.IsFalse(stream.TryRead(out byte[] _));
                }
            }

            [TestMethod]
            public void ReturnsWhatWriteChunkWrites()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(new byte[] { 0x01, 0x02, 0x03 });
                    stream.Position = 0;
                    Assert.IsTrue(stream.TryRead(out byte[] chunk));
                    Assert.IsTrue(chunk.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
                }
            }
        }
        
        [TestClass]
        public class WriteMethod
        {
            [TestMethod]
            public void PutsCorrectBytesInStream()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(new byte[] { 0x01, 0x02, 0x03 });
                    var data = stream.ToArray();
                    Assert.IsTrue(data.SequenceEqual(new byte[] { 0x03, 0x01, 0x02, 0x03 })); // We use Variable-length Quantity to encode how many bytes follow, hence the starting "0x03"
                }
            }

            [TestMethod]
            public void WritesNull()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write((byte[]) null);
                }
            }
        }
    }
}

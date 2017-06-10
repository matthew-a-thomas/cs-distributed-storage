namespace DistributedStorageTests.Storage
{
    using System.IO;
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorageFactoryClass
    {
        [TestClass]
        public class CreateMethod
        {
            [TestClass]
            public class ReturnValue
            {
                [TestClass]
                public class OurRsaKeyFileProperty
                {
                    [TestMethod]
                    public void CanBeReadFromAndWrittenToAndSeeked()
                    {
                        var storage = new StorageFactory().CreateStorage(Helpers.CreateDirectory());
                        Assert.IsTrue(storage.OurRsaKeyFile.TryOpenRead(out var stream));
                        using (stream)
                        {
                            Assert.IsTrue(stream.CanRead);
                            Assert.IsTrue(stream.CanSeek);
                            Assert.IsTrue(stream.CanWrite);
                            stream.WriteByte(0x12);
                            stream.Seek(0, SeekOrigin.Begin);
                            var b = stream.ReadByte();
                            Assert.AreEqual(0x12, b);
                        }
                    }
                }
            }
        }
    }
}

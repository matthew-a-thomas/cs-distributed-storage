namespace DistributedStorageTests.Common
{
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PipeClass
    {
        [TestClass]
        public class CreateLinkedPairMethod
        {
            [TestMethod]
            public void CanWriteToOneAndReadFromTheOther()
            {
                (var one, var two) = Pipe.CreateLinkedPair();
                one.WriteByte(0x01);
                var result = two.ReadByte();
                Assert.IsTrue(result == 1);
            }

            [TestMethod]
            public void DoesNotThrowException()
            {
                Pipe.CreateLinkedPair();
            }
        }
    }
}

namespace DistributedStorageTests
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DistributedStorage;

    [TestClass]
    public class ArrayExtensionsClass
    {
        [TestClass]
        public class AsBitsMethod
        {
            [TestMethod]
            public void UndoesAsBytesFor0Bits()
            {
                var bits = new bool[0];
                var bytes = bits.AsBytes();
                var bits2 = bytes.AsBits(0);
                Assert.AreEqual(0, bits2.Length);
            }

            [TestMethod]
            public void UndoesAsBytesFor8Bits()
            {
                var bits = Enumerable.Repeat(true, 8).ToArray();
                var bytes = bits.AsBytes();
                var bits2 = bytes.AsBits(8);
                Assert.AreEqual(8, bits2.Length);
                Assert.IsTrue(bits2.All(x => x));
            }

            [TestMethod]
            public void UndoesAsBytesFor9Bits()
            {
                var bits = new[] { true, true, true, false, true, false, true, false, true };
                var bytes = bits.AsBytes();
                var bits2 = bytes.AsBits(9);
                Assert.AreEqual(9, bits2.Length);
                Assert.IsTrue(bits2.Select((x, i) => x == bits[i]).All(x => x));
            }
        }

        [TestClass]
        public class AsBytesMethod
        {
            [TestMethod]
            public void ReturnsTheRightBytesFor8Bits()
            {
                var bits = Enumerable.Repeat(true, 8).ToArray();
                var bytes = bits.AsBytes();
                Assert.AreEqual(255, bytes[0]);
            }

            [TestMethod]
            public void ReturnsTheRightBytesFor9Bits()
            {
                var bits = new[] { true, true, true, false, true, false, true, false, true };
                var bytes = bits.AsBytes();
                Assert.AreEqual(0xEA, bytes[0]);
                Assert.AreEqual(0x80, bytes[1]);
            }

            [TestMethod]
            public void ReturnsTheRightNumberOfBytesFor0Bits()
            {
                var bits = new bool[0];
                var bytes = bits.AsBytes();
                Assert.AreEqual(0, bytes.Length);
            }

            [TestMethod]
            public void ReturnsTheRightNumberOfBytesFor8Bits()
            {
                var bits = new bool[8];
                var bytes = bits.AsBytes();
                Assert.AreEqual(1, bytes.Length);
            }

            [TestMethod]
            public void ReturnsTheRightNumberOfBytesFor9Bits()
            {
                var bits = new bool[9];
                var bytes = bits.AsBytes();
                Assert.AreEqual(2, bytes.Length);
            }
        }
    }
}

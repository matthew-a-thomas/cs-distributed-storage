namespace DistributedStorageTests.Common
{
    using System.Linq;
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestClass]
        public class CombineMethod
        {
            [TestMethod]
            public void PutsTwoBytesTogether()
            {
                var one = new byte[] { 0x01 };
                var two = new byte[] { 0x02 };
                var combined = new[] { one, two }.Combine();
                Assert.IsTrue(new byte[] { 0x01, 0x02 }.SequenceEqual(combined));
            }
        }

        [TestClass]
        public class SplitIntoMethod
        {
            [TestMethod]
            public void SplitsEvenlyDivisibleArray()
            {
                var array = new byte[] { 0x01, 0x02, 0x03, 0x04 };
                var split = array.SplitInto(2);
                Assert.IsTrue(split.Count == 2);
                Assert.IsTrue(split[0].SequenceEqual(new byte[] { 0x01, 0x02 }));
                Assert.IsTrue(split[1].SequenceEqual(new byte[] { 0x03, 0x04 }));
            }
        }

        [TestClass]
        public class SwapMethod
        {
            [TestMethod]
            public void SwapsTwoElements()
            {
                var array = new[] { 0, 1, 2 };
                array.Swap(0, 2);
                Assert.AreEqual(2, array[0]);
                Assert.AreEqual(0, array[2]);
            }
        }

        [TestClass]
        public class XorMethod
        {
            [TestMethod]
            public void XorsInAnotherBoolArray()
            {
                var array =    new[] { true, false, true };
                var with =     new[] { false, true, false };
                var expected = new[] { true, true, true };
                array.Xor(with);
                Assert.IsTrue(array.SequenceEqual(expected));
            }

            [TestMethod]
            public void XorsInAnotherByteArray()
            {
                var array = new byte[]    { 0b00000001, 0b00100000, 0b10000000 };
                var with =  new byte[]    { 0b00000010, 0b00010000, 0b10000000 };
                var expected = new byte[] { 0b00000011, 0b00110000, 0b00000000 };
                array.Xor(with);
                Assert.IsTrue(array.SequenceEqual(expected));
            }
        }
    }
}

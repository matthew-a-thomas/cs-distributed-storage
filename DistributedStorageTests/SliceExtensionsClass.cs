namespace DistributedStorageTests
{
    using System.IO;
    using System.Linq;
    using DistributedStorage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DistributedStorage.Serialization;

    [TestClass]
    public class SliceExtensionsClass
    {
        [TestClass]
        public class GetSliceMethod
        {
            [TestMethod]
            public void DeserializesWhatSerializeToWrites()
            {
                var slice = new Slice
                {
                    Coefficients = new[] { true, false, true, true, false, true, true },
                    EncodingSymbol = new byte[] { 0x01, 0x02, 0x03 }
                };
                Slice deserialized;
                using (var stream = new MemoryStream())
                {
                    slice.SerializeTo(stream);

                    stream.Flush();
                    stream.Position = 0;

                    deserialized = stream.GetSlice();
                }

                Assert.IsNotNull(deserialized);
                Assert.IsFalse(ReferenceEquals(deserialized, slice));
                Assert.IsNotNull(deserialized.Coefficients);
                Assert.IsFalse(ReferenceEquals(deserialized.Coefficients, slice.Coefficients));
                Assert.IsNotNull(deserialized.EncodingSymbol);
                Assert.IsFalse(ReferenceEquals(deserialized.EncodingSymbol, slice.EncodingSymbol));

                Assert.AreEqual(slice.Coefficients.Length, deserialized.Coefficients.Length);
                Assert.AreEqual(slice.EncodingSymbol.Length, deserialized.EncodingSymbol.Length);

                Assert.IsTrue(deserialized.Coefficients.Select((x, i) => x == slice.Coefficients[i]).All(x => x));
                Assert.IsTrue(deserialized.EncodingSymbol.Select((x, i) => x == slice.EncodingSymbol[i]).All(x => x));
            }
        }
    }
}

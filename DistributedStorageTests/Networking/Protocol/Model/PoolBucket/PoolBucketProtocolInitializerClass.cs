namespace DistributedStorageTests.Networking.Protocol.Model.PoolBucket
{
    using System;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Model;
    using DistributedStorage.Networking.Protocol.Model.PoolBucket;
    using DistributedStorage.Networking.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class PoolBucketProtocolInitializerClass
    {
        private static PoolBucketProtocolInitializer CreateInitializer()
        {
            var bytesToManifestAndSliceArrayTupleConverter = new Mock<IConverter<byte[], Tuple<Manifest, Slice[]>>>().Object;
            var nothingToBytesConverter = new Mock<IConverter<Nothing, byte[]>>().Object;
            var bytesToManifestAndHashArrayTupleConverter = new Mock<IConverter<byte[], Tuple<Manifest, Hash[]>>>().Object;
            var initializer = new PoolBucketProtocolInitializer(
                bytesToManifestAndSliceArrayTupleConverter,
                nothingToBytesConverter,
                bytesToManifestAndHashArrayTupleConverter
            );
            return initializer;
        }

        [TestClass]
        public class TrySetupMethod
        {
            [TestMethod]
            public void RegistersAllPublicPropertiesAndMethods()
            {
                var initializer = CreateInitializer();
                var poolBucket = new Mock<IPoolBucket>().Object;

                InitializerTest.RegistersAllPublicPropertiesAndMethods(initializer, poolBucket);
            }
        }
    }
}

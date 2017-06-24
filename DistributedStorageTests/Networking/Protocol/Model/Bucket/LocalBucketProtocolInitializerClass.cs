namespace DistributedStorageTests.Networking.Protocol.Model.Bucket
{
    using System;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Model;
    using DistributedStorage.Networking.Protocol.Model.Bucket;
    using DistributedStorage.Networking.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class LocalBucketProtocolInitializerClass
    {
        private static LocalBucketProtocolInitializer<IIdentity> CreateInitializer()
        {
            var bytesToNothingConverter = new Mock<IConverter<byte[], Nothing>>().Object;
            var longToBytesConverter = new Mock<IConverter<long, byte[]>>().Object;
            var bytesToManifestConverter = new Mock<IConverter<byte[], Manifest>>().Object;
            var hashArrayToBytesConverter = new Mock<IConverter<Hash[], byte[]>>().Object;
            var manifestArrayToBytesConverter = new Mock<IConverter<Manifest[], byte[]>>().Object;
            var bytesToManifestAndHashArrayTupleConverter = new Mock<IConverter<byte[], Tuple<Manifest, Hash[]>>>().Object;
            var sliceArrayToBytesConverter = new Mock<IConverter<Slice[], byte[]>>().Object;
            var tIdentityToBytesConverter = new Mock<IConverter<IIdentity, byte[]>>().Object;
            var initializer = new LocalBucketProtocolInitializer<IIdentity>(
                bytesToNothingConverter,
                longToBytesConverter,
                bytesToManifestConverter,
                hashArrayToBytesConverter,
                manifestArrayToBytesConverter,
                bytesToManifestAndHashArrayTupleConverter,
                sliceArrayToBytesConverter,
                tIdentityToBytesConverter
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
                var bucket = new Mock<IBucket<IIdentity>>().Object;

                InitializerTest.RegistersAllPublicPropertiesAndMethods(initializer, bucket);
            }

            [TestMethod]
            public void ReturnsTrue()
            {
                var initializer = CreateInitializer();
                var bucket = new Mock<IBucket<IIdentity>>().Object;
                var protocolMock = ProtocolHelper.CreateProtocolMock();
                Assert.IsTrue(initializer.TrySetup(protocolMock.Object, bucket));
            }
        }
    }
}

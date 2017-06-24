namespace DistributedStorageTests.Networking.Protocol.Model.Bucket
{
    using System;
    using System.Linq;
    using System.Reflection;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Model;
    using DistributedStorage.Networking.Protocol;
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

        private static Mock<IProtocol> CreateProtocolMock()
        {
            var mock = new Mock<IProtocol>();
            mock.Setup(x => x.TryRegister(It.IsAny<string>(), It.IsAny<IHandler<byte[], byte[]>>())).Returns(true);
            mock.Setup(x => x.TryUnregister(It.IsAny<string>())).Returns(true);
            return mock;
        }

        [TestClass]
        public class TrySetupMethod
        {
            [TestMethod]
            public void RegistersAllPublicPropertiesAndMethods()
            {
                var initializer = CreateInitializer();
                var bucket = new Mock<IBucket<IIdentity>>().Object;
                var protocolMock = CreateProtocolMock();
                Assert.IsTrue(initializer.TrySetup(bucket, protocolMock.Object));

                var type = typeof(IBucket<IIdentity>);
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                var members =
                    type
                    .GetMethods(bindingFlags)
                    .Where(method => !method.IsSpecialName) // Exclude property get_Methods https://stackoverflow.com/a/16238581/3063273
                    .Cast<MemberInfo>()
                    .Concat(type.GetProperties(bindingFlags));
                foreach (var member in members)
                {
                    protocolMock.Verify(x => x.TryRegister(member.Name, It.IsAny<IHandler<byte[], byte[]>>()), $"Property or method \"{member.Name}\" wasn't registered");
                }
            }

            [TestMethod]
            public void ReturnsTrue()
            {
                var initializer = CreateInitializer();
                var bucket = new Mock<IBucket<IIdentity>>().Object;
                var protocolMock = CreateProtocolMock();
                Assert.IsTrue(initializer.TrySetup(bucket, protocolMock.Object));
            }
        }
    }
}

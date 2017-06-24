namespace DistributedStorageTests.Networking.Protocol
{
    using DistributedStorage.Networking.Protocol;
    using Moq;

    internal static class ProtocolHelper
    {
        public static Mock<IProtocol> CreateProtocolMock()
        {
            var mock = new Mock<IProtocol>();
            mock.Setup(x => x.TryRegister(It.IsAny<string>(), It.IsAny<IHandler<byte[], byte[]>>())).Returns(true);
            mock.Setup(x => x.TryUnregister(It.IsAny<string>())).Returns(true);
            return mock;
        }
    }
}

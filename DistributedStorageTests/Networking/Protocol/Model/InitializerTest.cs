namespace DistributedStorageTests.Networking.Protocol.Model
{
    using System.Linq;
    using System.Reflection;
    using DistributedStorage.Networking.Protocol;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    internal static class InitializerTest
    {
        public static void RegistersAllPublicPropertiesAndMethods<T>(IProtocolInitializer<T> initializer, T with)
        {
            var protocolMock = ProtocolHelper.CreateProtocolMock();
            Assert.IsTrue(initializer.TrySetup(protocolMock.Object, with, out _));

            var type = typeof(T);
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
    }
}

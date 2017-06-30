namespace DistributedStorageTests.Networking.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Networking.Protocol;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public class GuidProtocolInitializerClass
    {
        private class ClassWithMethodWithInvalidGuid
        {
            [Guid("asdfsadf")]
            public void MethodWithInvalidGuid() { }
        }

        private class ClassWithMethodWithValidGuid
        {
            [Guid("97eed270-8a13-47be-b7f3-a02d27cfe7a3")]
            public void MethodWithValidGuid() { }
        }

        private class ClassWithValidGuidsAndDifferentMethodTypes
        {
            [Guid("c367abb7-eda8-4b76-84c3-664b5bffdb36")]
            public void MethodWithNoParametersAndNoReturnType() { }

            [Guid("31d087fd-94b5-4991-a64b-311bf4a5bf4d")]
            public void MethodWithParametersAndNoReturnType(object o) { }

            [Guid("d331d063-c626-4cb7-969d-458b95138ede")]
            public object MethodWithNoParametersButWithReturnType() => new object();

            [Guid("ece74f0c-e1a7-4567-9b08-778340574e22")]
            public int MethodWithParametersAndReturnType(double x) => (int)x;
        }

        [TestClass]
        public class TryCreateHandlerFactoryTuplesMethod
        {
            [TestMethod]
            public void CreatesListWithAllRequiredGuids()
            {
                Assert.IsTrue(GuidProtocolInitializer.TryCreateHandlerFactoryTuples<ClassWithValidGuidsAndDifferentMethodTypes>(_ => null, _ => null, out var tuples));
                var guids = new HashSet<Guid>(tuples.Select(x => x.Item1));
                Assert.IsTrue(guids.Remove(new Guid("c367abb7-eda8-4b76-84c3-664b5bffdb36")));
                Assert.IsTrue(guids.Remove(new Guid("31d087fd-94b5-4991-a64b-311bf4a5bf4d")));
                Assert.IsTrue(guids.Remove(new Guid("d331d063-c626-4cb7-969d-458b95138ede")));
                Assert.IsTrue(guids.Remove(new Guid("ece74f0c-e1a7-4567-9b08-778340574e22")));
                Assert.IsTrue(guids.Count == 0);
            }

            [TestMethod]
            public void RequestsTheRightSerializersAndDeserializers()
            {
                var requestedSerializerTypes = new HashSet<Type>();
                var requestedDeserializerTypes = new HashSet<Type>();
                Assert.IsTrue(GuidProtocolInitializer.TryCreateHandlerFactoryTuples<ClassWithValidGuidsAndDifferentMethodTypes>(
                    type => { requestedDeserializerTypes.Add(type); return null; },
                    type => { requestedSerializerTypes.Add(type); return null; },
                    out _));

                Assert.IsTrue(requestedDeserializerTypes.Remove(typeof(object)));
                Assert.IsTrue(requestedDeserializerTypes.Remove(typeof(double)));
                Assert.IsTrue(requestedDeserializerTypes.Count == 0);

                Assert.IsTrue(requestedSerializerTypes.Remove(typeof(object)));
                Assert.IsTrue(requestedSerializerTypes.Remove(typeof(int)));
                Assert.IsTrue(requestedSerializerTypes.Remove(typeof(void)));
                Assert.IsTrue(requestedSerializerTypes.Count == 0);
            }

            [TestMethod]
            public void ReturnsFalseWithMethodsWithInvalidGuids()
            {
                Assert.IsFalse(GuidProtocolInitializer.TryCreateHandlerFactoryTuples<ClassWithMethodWithInvalidGuid>(_ => null, _ => null, out _));
            }

            [TestMethod]
            public void ReturnsTrueWithMethodWithValidGuid()
            {
                Assert.IsTrue(GuidProtocolInitializer.TryCreateHandlerFactoryTuples<ClassWithMethodWithValidGuid>(_ => null, _ => null, out _));
            }

            [TestMethod]
            public void ReturnsTrueWithDifferentMethodTypes()
            {
                Assert.IsTrue(GuidProtocolInitializer.TryCreateHandlerFactoryTuples<ClassWithValidGuidsAndDifferentMethodTypes>(_ => null, _ => null, out _));
            }
        }

        [TestClass]
        public class TryCreateMethod
        {
            [TestMethod]
            public void ReturnsFalseWithMethodsWithInvalidGuids()
            {
                Assert.IsFalse(GuidProtocolInitializer.TryCreate<ClassWithMethodWithInvalidGuid>(_ => null, _ => null, out _));
            }

            [TestMethod]
            public void ReturnsTrueWithMethodWithValidGuid()
            {
                Assert.IsTrue(GuidProtocolInitializer.TryCreate<ClassWithMethodWithValidGuid>(_ => null, _ => null, out _));
            }

            [TestMethod]
            public void ReturnsTrueWithDifferentMethodTypes()
            {
                Assert.IsTrue(GuidProtocolInitializer.TryCreate<ClassWithValidGuidsAndDifferentMethodTypes>(_ => null, _ => null, out _));
            }
        }

        [TestClass]
        public class TrySetupMethod
        {
            [TestMethod]
            public void RegistersAllGivenHandlers()
            {
                Func<Guid, Func<object, IHandler<byte[], byte[]>>, Tuple<Guid, Func<object, IHandler<byte[], byte[]>>>> T = Tuple.Create;
                var guid1 = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                var initializer = new GuidProtocolInitializer<object>(new[]
                {
                    T(guid1, _ => null),
                    T(guid2, _ => null)
                });

                var protocolMock = new Mock<IProtocol>();
                protocolMock.Setup(x => x.TryRegister(It.IsAny<string>(), It.IsAny<IHandler<byte[], byte[]>>())).Returns(true);

                Assert.IsTrue(initializer.TrySetup(protocolMock.Object, new object(), out _));

                protocolMock.Verify(x => x.TryRegister(guid1.ToString(), It.IsAny<IHandler<byte[], byte[]>>()));
                protocolMock.Verify(x => x.TryRegister(guid2.ToString(), It.IsAny<IHandler<byte[], byte[]>>()));
            }
        }
    }
}

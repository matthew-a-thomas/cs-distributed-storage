namespace DistributedStorageTests.Networking
{
    using System;
    using System.Threading.Tasks;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking;
    using DistributedStorage.Networking.Protocol.Methods;
    using DistributedStorage.Networking.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class NodeClass
    {
        [TestClass]
        public class GetManifestsAsyncMethod
        {
            [TestMethod]
            public void InvokesGetManifestsMethod()
            {
                // Setup
                var getManifestsMethodMock = new Mock<IMethod<Nothing, Manifest[]>>();
                var getManifestsMethod = getManifestsMethodMock.Object;
                var getSliceCountForMethodMock = new Mock<IMethod<Manifest, int>>();
                var getSliceCountForMethod = getSliceCountForMethodMock.Object;
                var getSlicesForMethodMock = new Mock<IMethod<Manifest, Slice[]>>();
                var getSlicesForMethod = getSlicesForMethodMock.Object;
                var node = new Node(getManifestsMethod, getSliceCountForMethod, getSlicesForMethod);

                // Call
                node.GetManifestsAsync();

                // Assert
                getManifestsMethodMock.Verify(x => x.Invoke(It.IsAny<Nothing>(), It.IsAny<Action<Manifest[]>>()));
            }
        }

        [TestClass]
        public class GetSliceCountForMethod
        {
            [TestMethod]
            public void PassesAlongGivenManifest()
            {
                // Setup
                var getManifestsMethodMock = new Mock<IMethod<Nothing, Manifest[]>>();
                var getManifestsMethod = getManifestsMethodMock.Object;
                var getSliceCountForMethodMock = new Mock<IMethod<Manifest, int>>();
                var getSliceCountForMethod = getSliceCountForMethodMock.Object;
                var getSlicesForMethodMock = new Mock<IMethod<Manifest, Slice[]>>();
                var getSlicesForMethod = getSlicesForMethodMock.Object;
                var node = new Node(getManifestsMethod, getSliceCountForMethod, getSlicesForMethod);
                var manifest = new Manifest();

                // Call
                node.GetSliceCountFor(manifest);

                // Assert
                getSliceCountForMethodMock.Verify(x => x.Invoke(manifest, It.IsAny<Action<int>>()));
            }

            [TestMethod]
            public async Task ReturnsCount()
            {
                // Setup
                var getManifestsMethodMock = new Mock<IMethod<Nothing, Manifest[]>>();
                var getManifestsMethod = getManifestsMethodMock.Object;
                var getSliceCountForMethodMock = new Mock<IMethod<Manifest, int>>();
                const int numSlices = 1337;
                getSliceCountForMethodMock.Setup(x => x.Invoke(It.IsAny<Manifest>(), It.IsAny<Action<int>>())).Callback<Manifest, Action<int>>((parameter, callback) => callback(numSlices));
                var getSliceCountForMethod = getSliceCountForMethodMock.Object;
                var getSlicesForMethodMock = new Mock<IMethod<Manifest, Slice[]>>();
                var getSlicesForMethod = getSlicesForMethodMock.Object;
                var node = new Node(getManifestsMethod, getSliceCountForMethod, getSlicesForMethod);
                var manifest = new Manifest();

                // Call
                var count = await node.GetSliceCountFor(manifest);

                // Assert
                Assert.AreEqual(numSlices, count);
            }
        }

        [TestClass]
        public class GetSlicesForMethod
        {
            [TestMethod]
            public void PassesAlongGivenManifest()
            {
                // Setup
                var getManifestsMethodMock = new Mock<IMethod<Nothing, Manifest[]>>();
                var getManifestsMethod = getManifestsMethodMock.Object;
                var getSliceCountForMethodMock = new Mock<IMethod<Manifest, int>>();
                var getSliceCountForMethod = getSliceCountForMethodMock.Object;
                var getSlicesForMethodMock = new Mock<IMethod<Manifest, Slice[]>>();
                var getSlicesForMethod = getSlicesForMethodMock.Object;
                var node = new Node(getManifestsMethod, getSliceCountForMethod, getSlicesForMethod);
                var manifest = new Manifest();

                // Call
                node.GetSlicesFor(manifest);

                // Assert
                getSlicesForMethodMock.Verify(x => x.Invoke(manifest, It.IsAny<Action<Slice[]>>()));
            }

            [TestMethod]
            public async Task ReturnsSlices()
            {
                // Setup
                var getManifestsMethodMock = new Mock<IMethod<Nothing, Manifest[]>>();
                var getManifestsMethod = getManifestsMethodMock.Object;
                var getSliceCountForMethodMock = new Mock<IMethod<Manifest, int>>();
                var getSliceCountForMethod = getSliceCountForMethodMock.Object;
                var getSlicesForMethodMock = new Mock<IMethod<Manifest, Slice[]>>();
                var slices = new Slice[10];
                getSlicesForMethodMock.Setup(x => x.Invoke(It.IsAny<Manifest>(), It.IsAny<Action<Slice[]>>())).Callback<Manifest, Action<Slice[]>>((parameter, callback) => callback(slices));
                var getSlicesForMethod = getSlicesForMethodMock.Object;
                var node = new Node(getManifestsMethod, getSliceCountForMethod, getSlicesForMethod);
                var manifest = new Manifest();

                // Call
                var result = await node.GetSlicesFor(manifest);

                // Assert
                Assert.AreSame(slices, result);
            }
        }
    }
}

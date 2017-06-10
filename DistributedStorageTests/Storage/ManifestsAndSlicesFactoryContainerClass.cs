namespace DistributedStorageTests.Storage
{
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.FileSystem;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ManifestsAndSlicesFactoryContainerClass
    {
        private static void Create(out IDirectory directory, out ManifestsAndSlicesFactoryContainer container)
        {
            directory = Helpers.CreateDirectory();
            var options = new ManifestsAndSlicesFactoryContainer.Options(".manifest", ".slice", directory);
            container = new ManifestsAndSlicesFactoryContainer(options);
        }

        [TestClass]
        public class GetKeysMethod
        {
            [TestMethod]
            public void ReturnsNothingForNewInstance()
            {
                Create(out _, out var container);
                Assert.IsFalse(container.GetKeys().Any());
            }
        }

        [TestClass]
        public class TryCreateMethod
        {
            [TestMethod]
            public void ReturnsNewSliceContainer()
            {
                Create(out _, out var container);
                var manifest = new Manifest
                {
                    Id = Hash.Create(System.Text.Encoding.ASCII.GetBytes("manifest")),
                    Length = 0,
                    SliceHashes = new Hash[0]
                };
                Assert.IsTrue(container.TryCreate(manifest, out _));
            }
        }

        [TestClass]
        public class TryGetMethod
        {
            [TestMethod]
            public void GetsSliceContainerForExistentManifest()
            {
                Create(out _, out var container);
                var manifest = new Manifest
                {
                    Id = Hash.Create(System.Text.Encoding.ASCII.GetBytes("manifest")),
                    Length = 0,
                    SliceHashes = new Hash[0]
                };
                Assert.IsTrue(container.TryCreate(manifest, out _));
                Assert.IsTrue(container.TryGet(manifest, out _));
            }

            [TestMethod]
            public void ReturnsFalseForNonexistentManifest()
            {
                Create(out _, out var container);
                var manifest = new Manifest
                {
                    Id = Hash.Create(System.Text.Encoding.ASCII.GetBytes("manifest")),
                    Length = 0,
                    SliceHashes = new Hash[0]
                };
                Assert.IsFalse(container.TryGet(manifest, out _));
            }
        }

        [TestClass]
        public class TryRemoveMethod
        {
            [TestMethod]
            public void ReturnsFalseForNonexistentManifest()
            {
                Create(out _, out var container);
                var manifest = new Manifest
                {
                    Id = Hash.Create(System.Text.Encoding.ASCII.GetBytes("manifest")),
                    Length = 0,
                    SliceHashes = new Hash[0]
                };
                Assert.IsFalse(container.TryRemove(manifest));
            }

            [TestMethod]
            public void ReturnsTrueForExistentManifest()
            {
                Create(out _, out var container);
                var manifest = new Manifest
                {
                    Id = Hash.Create(System.Text.Encoding.ASCII.GetBytes("manifest")),
                    Length = 0,
                    SliceHashes = new Hash[0]
                };
                Assert.IsTrue(container.TryCreate(manifest, out _));
                Assert.IsTrue(container.TryRemove(manifest));
            }
        }
    }
}

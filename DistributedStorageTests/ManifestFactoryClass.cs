namespace DistributedStorageTests
{
    using System;
    using System.Security.Cryptography;
    using DistributedStorage.Encoding;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ManifestFactoryClass
    {
        private static readonly Func<HashAlgorithm> HasherFactory = SHA256.Create;

        [TestClass]
        public class CreateFromMethod
        {
            [TestMethod]
            public void CreatesValidManifest()
            {
                var factory = new ManifestFactory(HasherFactory);
                var manifest = factory.CreateManifestFrom(new byte[] { 1, 2, 3, 4 }, 2);
                Assert.AreEqual(4, manifest.Length);
                Assert.IsNotNull(manifest.SliceHashes);
                Assert.AreEqual(2, manifest.SliceHashes.Length);
                Assert.IsNotNull(manifest.Id);
            }

            [TestMethod]
            public void CreatesValidManifestWhenNotAligned()
            {
                var factory = new ManifestFactory(HasherFactory);
                var manifest = factory.CreateManifestFrom(new byte[] { 1, 2, 3 }, 2);
                Assert.AreEqual(3, manifest.Length);
                Assert.IsNotNull(manifest.SliceHashes);
                Assert.AreEqual(2, manifest.SliceHashes.Length);
                Assert.IsNotNull(manifest.Id);
            }
        }
    }
}

namespace DistributedStorageTests.Serialization
{
    using System;
    using System.IO;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ManifestExtensionsClass
    {
        [TestClass]
        public class GetManifestMethod
        {
            [TestMethod]
            public void DeserializesWhatSerializeToWrites()
            {
                var hash = new Hash();
                hash.HashCode[0] = 0b10101010;
                hash.HashCode[hash.HashCode.Length - 1] = 0b11110000;
                var manifest = new Manifest
                {
                    Id = hash,
                    Length = 1337,
                    SliceHashes = new[] {hash, hash}
                };
                Manifest deserialized;
                using (var stream = new MemoryStream())
                {
                    stream.WriteManifest(manifest);

                    stream.Flush();
                    stream.Position = 0;

                    if (!stream.TryReadManifest(TimeSpan.FromSeconds(1), out deserialized))
                        throw new Exception("Couldn't read manifest");
                }

                // Make sure we got something back that is completely distinct from what we put in
                Assert.IsNotNull(deserialized);
                Assert.IsFalse(ReferenceEquals(deserialized, manifest));
                Assert.IsNotNull(deserialized.Id);
                Assert.IsFalse(ReferenceEquals(hash, deserialized.Id));
                Assert.IsNotNull(deserialized.SliceHashes);
                Assert.IsFalse(deserialized.SliceHashes.Any(x => ReferenceEquals(null, x)));
                Assert.IsFalse(deserialized.SliceHashes.Any(x => ReferenceEquals(x, hash)));

                // Make sure the ID is the same
                Assert.IsTrue(hash.HashCode.SequenceEqual(deserialized.Id.HashCode));

                // Make sure the length is the same
                Assert.AreEqual(1337, deserialized.Length);

                // Make sure the slice hashes are the same
                Assert.IsTrue(deserialized.SliceHashes.Length == manifest.SliceHashes.Length);
                Assert.IsTrue(deserialized.SliceHashes.Select(x => x.HashCode.SequenceEqual(hash.HashCode)).All(x => x));
            }
        }
    }
}

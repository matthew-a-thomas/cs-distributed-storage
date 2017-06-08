namespace DistributedStorageTests.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using File = DistributedStorage.Storage.File;

    [TestClass]
    public class FileSliceContainerClass
    {
        /// <summary>
        /// Creates a new <see cref="FileSliceContainer"/> and <see cref="IFactoryContainer{TKey, TValue}"/> that operate from in-memory structures
        /// </summary>
        private static void CreateContainerAndFolder(out FileSliceContainer container, out IFactoryContainer<string, IFile> folder)
        {
            var dictionaryContainer = new Dictionary<string, IFile>().ToContainer();
            bool TryCreate(string key, out IFile file)
            {
                if (dictionaryContainer.TryGet(key, out file))
                    return false;
                file = CreateFile();
                return dictionaryContainer.TryAdd(key, file);
            }
            folder = new FactoryContainer<string, IFile>(dictionaryContainer, new FactoryContainer<string, IFile>.Options
            {
                TryCreate = TryCreate
            });
            container = new FileSliceContainer(folder);
        }

        /// <summary>
        /// Creates a new <see cref="IFile"/> that wraps an internal byte array
        /// </summary>
        private static IFile CreateFile()
        {
            var buffer = new byte[1024 * 10];

            bool TryOpen(out Stream stream)
            {
                stream = new MemoryStream(buffer);
                return true;
            }

            return new File(new File.Options
            {
                TryOpenRead = TryOpen,
                TryOpenWrite = TryOpen
            });
        }

        private static Slice CreateSlice() => new Slice
        {
            Coefficients = new[] {true, false, false},
            EncodingSymbol = new byte[] {0x00, 0x01, 0x02}
        };

        [TestClass]
        public class GetKeysMethod
        {
            [TestMethod]
            public void DoesNotReturnNonHexNames()
            {
                CreateContainerAndFolder(out var container, out var folder);
                Assert.IsTrue(folder.TryCreate("hello world", out _));
                Assert.IsFalse(container.GetKeys().Any());
            }

            [TestMethod]
            public void DoesReturnHashName()
            {
                CreateContainerAndFolder(out var container, out var folder);
                var hash = Hash.Create(System.Text.Encoding.ASCII.GetBytes("Hello world"));
                Assert.IsTrue(folder.TryCreate(hash.HashCode.ToHex(), out _));
                Assert.IsTrue(container.GetKeys().Count() == 1);
            }
        }

        [TestClass]
        public class TryAddMethod
        {
            [TestMethod]
            public void ReturnsFalseForDuplicateHash()
            {
                CreateContainerAndFolder(out var container, out var folder);
                var hash = Hash.Create(System.Text.Encoding.ASCII.GetBytes("Hello world"));
                Assert.IsTrue(folder.TryAdd(hash.HashCode.ToHex(), CreateFile()));
                Assert.IsFalse(container.TryAdd(hash, CreateSlice()));
            }

            [TestMethod]
            public void ReturnsTrueForUniqueHash()
            {
                CreateContainerAndFolder(out var container, out _);
                Assert.IsTrue(container.TryAdd(Hash.Create(new byte[0]), CreateSlice()));
            }
        }

        [TestClass]
        public class TryGetMethod
        {
            [TestMethod]
            public void Implemented() => throw new NotImplementedException();
        }

        [TestClass]
        public class TryRemoveMethod
        {
            [TestMethod]
            public void Implemented() => throw new NotImplementedException();
        }
    }
}

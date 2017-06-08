namespace DistributedStorageTests.Storage
{
    using DistributedStorage.Storage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FileSliceContainerClass
    {
        private static void CreateContainer(out FileSliceContainer container, out IFactoryContainer<string, IFile> folder)
        {
            folder = new FactoryContainer<string, IFile>(
                container: new Container<string, IFile>(new Container<string, IFile>.Options
                {
                    GetKeys
                }),
                options: new FactoryContainer<string, IFile>.Options
                {

                });
            container = new FileSliceContainer(workingFolder: folder);
        }

        [TestClass]
        public class GetKeysMethod
        { }

        [TestClass]
        public class TryAddMethod
        { }

        [TestClass]
        public class TryGetMethod
        { }

        [TestClass]
        public class TryRemoveMethod
        { }
    }
}

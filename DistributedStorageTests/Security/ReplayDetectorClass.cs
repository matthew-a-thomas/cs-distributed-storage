namespace DistributedStorageTests.Security
{
    using DistributedStorage.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReplayDetectorClass
    {
        [TestClass]
        public class CleanMethod
        {
            [TestMethod]
            public void DoesNotRemoveNewObject()
            {
                var detector = new ReplayDetector<object>();
                var obj = new object();
                detector.TryAdd(obj, 0);
                detector.Clean(-1);
                Assert.IsFalse(detector.TryAdd(obj, 1));
            }

            [TestMethod]
            public void RemovesOldObject()
            {
                var detector = new ReplayDetector<object>();
                var obj = new object();
                detector.TryAdd(obj, 0);
                detector.Clean(0);
                Assert.IsTrue(detector.TryAdd(obj, 1));
            }
        }

        [TestClass]
        public class TryAddMethod
        {
            [TestMethod]
            public void ReturnsTrueForFirstObject()
            {
                Assert.IsTrue(new ReplayDetector<object>().TryAdd(new object(), 0));
            }

            [TestMethod]
            public void ReturnsFalseForDuplicateObject()
            {
                var obj = new object();
                var detector = new ReplayDetector<object>();
                detector.TryAdd(obj, 0);
                Assert.IsFalse(detector.TryAdd(obj, 1));
            }

            [TestMethod]
            public void ReturnsFalseForOldTag()
            {
                var detector = new ReplayDetector<object>();
                detector.TryAdd(new object(), 0);
                detector.TryAdd(new object(), -1);
            }
        }
    }
}

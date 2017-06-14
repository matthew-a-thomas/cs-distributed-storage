namespace DistributedStorageTests.Networking.Security
{
    using DistributedStorage.Networking.Security;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RsaParametersExtensionsClass
    {
        private static RsaKeyProvider KeyProvider = new RsaKeyProvider();

        [TestClass]
        public class TryReadMethod
        {
            [TestMethod]
            public void ReadsPrivateParametersThatWereWrittenOut()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(KeyProvider.RsaKey1, true);

                    stream.Position = 0;

                    Assert.IsTrue(stream.TryRead(out var key));

                    Assert.IsNotNull(key.D);
                    Assert.IsNotNull(key.DP);
                    Assert.IsNotNull(key.DQ);
                    Assert.IsNotNull(key.Exponent);
                    Assert.IsNotNull(key.InverseQ);
                    Assert.IsNotNull(key.Modulus);
                    Assert.IsNotNull(key.P);
                    Assert.IsNotNull(key.Q);
                }
            }

            [TestMethod]
            public void ReadsOnlyPublicParametersThatWereWrittenOut()
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(KeyProvider.RsaKey1, false);

                    stream.Position = 0;

                    Assert.IsTrue(stream.TryRead(out var key));

                    Assert.IsNull(key.D);
                    Assert.IsNull(key.DP);
                    Assert.IsNull(key.DQ);
                    Assert.IsNotNull(key.Exponent);
                    Assert.IsNull(key.InverseQ);
                    Assert.IsNotNull(key.Modulus);
                    Assert.IsNull(key.P);
                    Assert.IsNull(key.Q);
                }
            }
        }
    }
}

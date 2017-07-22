namespace DistributedStorageTests.Storage.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DistributedStorage.Storage.FileSystem;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FileSystemSafeStringAdapterClass
    {
        [TestMethod]
        public void ConvertsFromRawToSafeBackToRawForLotsOfRandomStrings()
        {
            var random = new Random(0);
            for (var i = 0; i < 10000; ++i)
            {
                var randomString = GenerateRandomString(random);
                
                var safe = FileSystemSafeStringAdapter.MakeSafe(randomString);
                var raw = FileSystemSafeStringAdapter.MakeRaw(safe);

                Assert.AreEqual(randomString, raw);
            }
        }

        [TestMethod]
        public void MakesSpecificStringsSafe()
        {
            var badCharacters = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToArray();
            
            foreach (var testString in new[]
            {
                @"/../../..",
                @"\..\..\.."
            })
            {
                var goodString = FileSystemSafeStringAdapter.MakeSafe(testString);
                foreach (var badCharacter in badCharacters)
                {
                    Assert.IsFalse(goodString.Contains(badCharacter), $"The \"safe\" string \"{goodString}\" contains the bad character \"{badCharacter}\"");
                }
            }
        }

        [TestMethod]
        public void ProducesExpectedRawStringFromSafeStringWithInterleavedEscapeCharacters()
        {
            var safeString = $@"{FileSystemSafeStringAdapter.EscapeChar}32{FileSystemSafeStringAdapter.EscapeChar}32{FileSystemSafeStringAdapter.EscapeChar}32{FileSystemSafeStringAdapter.EscapeChar}";
            var rawString = FileSystemSafeStringAdapter.MakeRaw(safeString);
            Assert.AreEqual(@" 32 ", rawString);
        }
        
        private static string GenerateRandomString(Random random)
        {
            var stringBuilder = new StringBuilder();
            foreach (var randomNumber in Enumerable.Repeat(0, 100).Select(_ => random.Next()))
            {
                if (randomNumber == 0)
                    continue;
                stringBuilder.Append((char) randomNumber);
            }
            var randomString = stringBuilder.ToString();
            return randomString;
        }
    }
}

namespace DistributedStorageTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeneratorClass
    {
        [TestClass]
        public class NextMethod
        {
            [TestMethod]
            public void CorrectlyCombinesSourceSymbols()
            {
                const byte
                    a = 13,
                    b = 37,
                    c = 17;
                var source = new List<byte[]>
                {
                    new [] { a },
                    new [] { b },
                    new [] { c }
                };
                var random = new RandomAdapter(new Random(0));
                var generator = new Generator(random, source);
                for (var iteration = 0; iteration < 100; ++iteration)
                {
                    var slice = generator.Next();
                    var coefficients = slice.Coefficients;
                    var encodingSymbol = slice.EncodingSymbol;

                    Assert.IsNotNull(coefficients);
                    Assert.IsNotNull(encodingSymbol);
                    Assert.AreEqual(3, coefficients.Length);
                    Assert.AreEqual(1, encodingSymbol.Length);

                    Assert.IsTrue(coefficients[0] | coefficients[1] | coefficients[2]);

                    var expectedOutput = (byte)((coefficients[0] ? a : 0) ^ (coefficients[1] ? b : 0) ^ (coefficients[2] ? c : 0));
                    Assert.AreEqual(expectedOutput, encodingSymbol[0]);
                }
            }

            [TestMethod]
            public void EventuallyGeneratesAllCombinationsOfCoefficients()
            {
                const byte
                    a = 13,
                    b = 37,
                    c = 17;
                var source = new List<byte[]>
                {
                    new [] { a },
                    new [] { b },
                    new [] { c }
                };
                var random = new RandomAdapter(new Random(0));
                var generator = new Generator(random, source);
                var remainingNumbers = new HashSet<int>(Enumerable.Range(1, 7));
                for (var iteration = 0; iteration < 100 && remainingNumbers.Count > 0; ++iteration)
                {
                    var slice = generator.Next();
                    var coefficients = slice.Coefficients;
                    var number = 0;
                    if (coefficients[0])
                        number += 1;
                    if (coefficients[1])
                        number += 2;
                    if (coefficients[2])
                        number += 4;
                    remainingNumbers.Remove(number);
                }
                Assert.AreEqual(0, remainingNumbers.Count);
            }

            /// <summary>
            /// A "systematic" generator will first output the source symbols in order before it begins combining them together
            /// </summary>
            [TestMethod]
            public void IsSystematic()
            {
                const byte
                    a = 13,
                    b = 37,
                    c = 17;
                var source = new List<byte[]>
                {
                    new [] { a },
                    new [] { b },
                    new [] { c }
                };
                var random = new RandomAdapter(new Random(0));
                var generator = new Generator(random, source);
                foreach (var symbol in source)
                {
                    var slice = generator.Next();
                    var coefficients = slice.Coefficients;
                    var encodingSymbol = slice.EncodingSymbol;

                    var countOfBits = coefficients.Count(bit => bit);
                    Assert.AreEqual(1, countOfBits);
                    Assert.AreEqual(symbol[0], encodingSymbol[0]);
                }
            }
        }
    }
}

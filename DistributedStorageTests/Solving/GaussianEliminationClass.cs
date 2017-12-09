namespace DistributedStorageTests.Solving
{
    using System.Collections.Generic;
    using Matt.Math.Linear.Solving;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GaussianEliminationClass
    {
        [TestClass]
        public class SolveMethod
        {
            [TestMethod]
            public void DoesNotSolveUnsolvableSystem()
            {
                var coefficients = new List<bool[]>
                {
                    new[] { true, false, false }
                };
                var solutions = new List<byte[]>
                {
                    new[] {(byte) 'C'},
                    new[] {(byte) 'a'},
                    new[] {(byte) 't'}
                };
                var gaussian = new GaussianElimination(coefficients, solutions);
                var solution = gaussian.Solve();

                Assert.IsNull(solution);
            }

            [TestMethod]
            public void SolvesAlreadySolvedSystem()
            {
                var coefficients = new List<bool[]>
                {
                    new[] { true, false, false },
                    new[] { false, true, false },
                    new[] { false, false, true }
                };
                var solutions = new List<byte[]>
                {
                    new[] {(byte) 'C'},
                    new[] {(byte) 'a'},
                    new[] {(byte) 't'}
                };
                var gaussian = new GaussianElimination(coefficients, solutions);
                var solution = gaussian.Solve();

                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.Count);
                Assert.AreEqual((byte)'C', solution[0][0]);
                Assert.AreEqual((byte)'a', solution[1][0]);
                Assert.AreEqual((byte)'t', solution[2][0]);
            }

            [TestMethod]
            public void SolvesEasilySolvableSystem()
            {
                var coefficients = new List<bool[]>
                {
                    new[] { false, false, true },
                    new[] { false, true, false },
                    new[] { true, false, false }
                };
                var solutions = new List<byte[]>
                {
                    new[] {(byte) 't'},
                    new[] {(byte) 'a'},
                    new[] {(byte) 'C'}
                };
                var gaussian = new GaussianElimination(coefficients, solutions);
                var solution = gaussian.Solve();

                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.Count);
                Assert.AreEqual((byte)'C', solution[0][0]);
                Assert.AreEqual((byte)'a', solution[1][0]);
                Assert.AreEqual((byte)'t', solution[2][0]);
            }

            [TestMethod]
            public void SolvesComplicatedSystem()
            {
                const byte
                    a = 13,
                    b = 37,
                    c = 17;
                var coefficients = new List<bool[]>
                {
                    new[] {true, false, true},
                    new[] {true, true, true},
                    new[] {false, false, true}
                };
                var solutions = new List<byte[]>
                {
                    new byte[] {a ^ c},
                    new byte[] {a ^ b ^ c},
                    new [] {c}
                };
                var gaussian = new GaussianElimination(coefficients, solutions);
                var solution = gaussian.Solve();

                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.Count);
                Assert.AreEqual(a, solution[0][0]);
                Assert.AreEqual(b, solution[1][0]);
                Assert.AreEqual(c, solution[2][0]);
            }

            [TestMethod]
            public void SolvesOverlySolvedSystem()
            {
                var coefficients = new List<bool[]>
                {
                    new[] { false, false, true },
                    new[] { false, true, false },
                    new[] { true, false, false },
                    new[] { false, false, true },
                    new[] { false, true, false },
                    new[] { true, false, false }
                };
                var solutions = new List<byte[]>
                {
                    new[] {(byte) 't'},
                    new[] {(byte) 'a'},
                    new[] {(byte) 'C'},
                    new[] {(byte) 't'},
                    new[] {(byte) 'a'},
                    new[] {(byte) 'C'}
                };
                var gaussian = new GaussianElimination(coefficients, solutions);
                var solution = gaussian.Solve();

                Assert.IsNotNull(solution);
                Assert.AreEqual(3, solution.Count);
                Assert.AreEqual((byte)'C', solution[0][0]);
                Assert.AreEqual((byte)'a', solution[1][0]);
                Assert.AreEqual((byte)'t', solution[2][0]);
            }
        }
    }
}

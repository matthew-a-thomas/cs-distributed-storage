namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using DistributedStorage;

    // ReSharper disable once UnusedMember.Global
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            using (var distributedStorage = new DistributedStorage())
            {
                const int numSlices = 5;
                var data = Encoding.ASCII.GetBytes("Hello world!");
                var manifest = distributedStorage.CreateManifestFrom(data, numSlices);
                var parts = data.SplitInto(numSlices);
                var generator = distributedStorage.CreateGeneratorFor(parts);
                var solver = distributedStorage.CreateSolverFor(manifest);
                
                byte[] solution = null;
                var solved = false;
                while (!solved && solution == null)
                {
                    var slice = generator.Next();

                    "Generated slice.".Choose(new Dictionary<string, Action>
                    {
                        { "Use it", () => solved = solver.TrySolve(slice, out solution) },
                        {
                            "Scramble it",
                            () =>
                            {
                                for (var i = 0; i < slice.EncodingSymbol.Length; ++i)
                                    slice.EncodingSymbol[i] += 2;
                                solved = solver.TrySolve(slice, out solution);
                            }
                        },
                        { "Toss it", () => { } }
                    });
                }

                (solution == null ? "Solved!" : "Corrupt").Say();
                Encoding.ASCII.GetString(solution).Say();

                Console.ReadKey(true);
            }
        }
    }
}
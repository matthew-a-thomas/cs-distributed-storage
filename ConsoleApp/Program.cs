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
                while (true)
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

                    (solution != null && solved ? "Solved!" : solution != null ? "Corrupt" : "Not solved").Say();
                    Encoding.ASCII.GetString(solution ?? new byte[0]).Say();
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
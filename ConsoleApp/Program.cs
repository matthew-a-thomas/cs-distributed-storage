using Newtonsoft.Json;

namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using DistributedStorage;
    using DistributedStorage.Serialization;

    // ReSharper disable once UnusedMember.Global
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private static void Main()
        {
            "What to do?".Choose(new Dictionary<string, Action>
            {
                {
                    "Play with individual slices",
                    () =>
                    {
                        using (var distributedStorage = new DistributedStorage())
                        {
                            var generatorFactory = distributedStorage.GeneratorFactory;
                            var manifestFactory = distributedStorage.ManifestFactory;
                            var solverFactory = distributedStorage.SolverFactory;

                            const int numSlices = 5;
                            var data = Encoding.ASCII.GetBytes("Hello world!");
                            var manifest = manifestFactory.CreateManifestFrom(data, numSlices);
                            var parts = data.SplitInto(numSlices);
                            var generator = generatorFactory.CreateGeneratorFor(parts);
                            var solver = solverFactory.CreateSolverFor(manifest);

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
                    }
                },
                {
                    "Generate slices from file",
                    () =>
                    {
                        $"You are currently in {Directory.GetCurrentDirectory()}".Say();

                        var filename = "File name?".Ask();
                        "Reading...".Say();
                        var data = File.ReadAllBytes(filename);
                        $"Read {data.Length} bytes".Say();

                        if (!int.TryParse("How many chunks?".Ask(), out var numChunks))
                            numChunks = 10;
                        $"Will use {numChunks} chunks".Say();

                        using (var distributedStorage = new DistributedStorage())
                        {
                            var generatorFactory = distributedStorage.GeneratorFactory;
                            var manifestFactory = distributedStorage.ManifestFactory;

                            "Generating manifest...".Say();
                            var manifest = manifestFactory.CreateManifestFrom(data, numChunks);
                            var manifestOutputFileName = $"{filename}.manifest";
                            using (var manifestOutputFile = File.OpenWrite(manifestOutputFileName))
                            {
                                manifest.SerializeTo(manifestOutputFile);
                            }
                            manifestOutputFileName.Say();

                            "Splitting data into chunks...".Say();
                            var parts = data.SplitInto(numChunks);

                            "Creating slice generator...".Say();
                            var generator = generatorFactory.CreateGeneratorFor(parts);

                            for (var i = 0; ; ++i)
                            {
                                "Press any key to generate a slice...".Wait();

                                var slice = generator.Next();
                                var outputFilename = $"{filename}.{i}";
                                using (var stream = File.OpenWrite(outputFilename))
                                    slice.SerializeTo(stream);

                                outputFilename.Say();
                            }
                        }
                    }
                },
                {
                    "Read slices",
                    () =>
                    {
                        $"You are currently in {Directory.GetCurrentDirectory()}".Say();
                        var manifestFileInfo = new FileInfo("Manifest?".Ask());
                        Manifest manifest;
                        using (var stream = manifestFileInfo.OpenRead())
                        {
                            manifest = stream.GetManifest();
                        }

                        using (var distributedStorage = new DistributedStorage())
                        {
                            var solverFactory = distributedStorage.SolverFactory;

                            var solver = solverFactory.CreateSolverFor(manifest);
                            var solution = default(byte[]);
                            var solved = false;
                            var originalFileName = Path.GetFileNameWithoutExtension(manifestFileInfo.Name);
                            foreach (var file in
                                manifestFileInfo
                                .Directory
                                .EnumerateFiles($"{originalFileName}.*")
                            )
                            {
                                if (!int.TryParse(file.Extension.Substring(1), out _))
                                    continue;
                                $"Using {file.Name}...".Say();
                                using (var stream = file.OpenRead())
                                {
                                    var slice = stream.GetSlice();

                                    // ReSharper disable once AssignmentInConditionalExpression
                                    if (solved = solver.TrySolve(slice, out solution))
                                        break;
                                }
                            }
                            if (solution == null)
                            {
                                "Not enough slices available".Say();
                            }
                            else if (!solved)
                            {
                                "Corrupted".Say();
                            }
                            else
                            {
                                "Valid".Say();
                            }
                            JsonConvert.SerializeObject(solution ?? new byte[0]).Say();
                        }
                    }
                }
            });
            "Press any key to exit . . .".Wait();
        }
    }
}
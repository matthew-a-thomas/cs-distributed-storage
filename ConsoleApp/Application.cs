﻿namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using DistributedStorage;
    using DistributedStorage.Serialization;
    using Newtonsoft.Json;
    using Common;
    using Security;

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    internal class Application
    {
        #region Private fields

        /// <summary>
        /// Creates visual representations of hash codes
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        private readonly HashVisualizer _hashVisualizer;

        /// <summary>
        /// Generates secure streams
        /// </summary>
        private readonly SecureStreamFactory _secureStreamFactory;

        private readonly ICryptoRsa _cryptoRsa;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="Application"/>
        /// </summary>
        public Application(
            HashVisualizer hashVisualizer,
            SecureStreamFactory secureStreamFactory,
            ICryptoRsa cryptoRsa)
        {
            _hashVisualizer = hashVisualizer;
            _secureStreamFactory = secureStreamFactory;
            _cryptoRsa = cryptoRsa;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Guides the user through a visual representation of a hash code
        /// </summary>
        private void DisplayHashCode()
        {
            var message = "Message to hash?".Ask();
            var data = Encoding.ASCII.GetBytes(message);
            FileInfo destination;
            using (var bitmap = _hashVisualizer.MakeBitmap(data, 2048))
            {
                $"You are currently in {Directory.GetCurrentDirectory()}".Say();
                destination = new FileInfo("Destination file name?".Ask());
                bitmap.Save(destination.FullName);
            }
            "Showing image...".Say();
            new Launcher().Launch(destination);
        }

        /// <summary>
        /// Guides the user through generating slices from a chosen file
        /// </summary>
        private static void GenerateSlicesFromFile()
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

        /// <summary>
        /// Guides the user through a simulation of sending slices over a lossy communications channel
        /// </summary>
        private static void PlayWithSlices()
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
                        {"Use it", () => solved = solver.TrySolve(slice, out solution)},
                        {
                            "Scramble it",
                            () =>
                            {
                                for (var i = 0; i < slice.EncodingSymbol.Length; ++i)
                                    slice.EncodingSymbol[i] += 2;
                                solved = solver.TrySolve(slice, out solution);
                            }
                        },
                        {"Toss it", () => { }}
                    });

                    (solution != null && solved ? "Solved!" : solution != null ? "Corrupt" : "Not solved").Say();
                    Encoding.ASCII.GetString(solution ?? new byte[0]).Say();
                }
            }
        }

        /// <summary>.com
        /// Guides the user through recreating a file from slices
        /// </summary>
        private static void ReadSlices()
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

        /// <summary>
        /// Runs this <see cref="Application"/>
        /// </summary>
        public void Run()
        {
            "What to do?".Choose(new Dictionary<string, Action>
            {
                {
                    "Play with individual slices",
                    PlayWithSlices
                },
                {
                    "Generate slices from file",
                    GenerateSlicesFromFile
                },
                {
                    "Read slices",
                    ReadSlices
                },
                {
                    "Display hash code",
                    DisplayHashCode
                },
                {
                    "Connect",
                    () =>
                    {
                        "Generating an RSA key...".Say();
                        var key = _cryptoRsa.CreateKey();
                        $"Your RSA key has this fingerprint: {key.ComputeHashCode().ToHex()}".Say();
                        "Mode?".Choose(new Dictionary<string, Action>
                        {
                            {
                                "Make", () =>
                                {
                                    using (var client = new TcpClient())
                                    {
                                        client.ConnectAsync(IPAddress.Loopback, 1337).Wait();
                                        using (var stream = client.GetStream())
                                        {
                                            if (!_secureStreamFactory.TryCreateConnection(stream, key, SecureStreamFactory.Mode.Make, TimeSpan.FromSeconds(1), out var theirs, out var secureStream))
                                            {
                                                "Failed to connect".Say();
                                                return;
                                            }
                                            var accept = false;
                                            theirs.ComputeHashCode().ToHex().Choose(new Dictionary<string, Action>
                                            {
                                                { "Accept", () => accept = true },
                                                { "Reject", () => accept = false }
                                            });
                                            if (!accept)
                                                return;
                                            while (true)
                                            {
                                                var message = "Message?".Ask();
                                                var datagram = Encoding.ASCII.GetBytes(message);
                                                secureStream.SendDatagram(datagram);
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                "Accept",
                                () =>
                                {
                                    var listener = new TcpListener(IPAddress.Loopback, 1337);
                                    listener.Start();
                                    var acceptTask = listener.AcceptTcpClientAsync();
                                    acceptTask.Wait();
                                    using (var client = acceptTask.Result)
                                    {
                                        using (var stream = client.GetStream())
                                        {
                                            if (!_secureStreamFactory.TryCreateConnection(stream, key, SecureStreamFactory.Mode.Accept, TimeSpan.FromSeconds(1), out var theirs, out var secureStream))
                                            {
                                                "Failed to accept a connection".Say();
                                                return;
                                            }
                                            var accept = false;
                                            theirs.ComputeHashCode().ToHex().Choose(new Dictionary<string, Action>
                                            {
                                                { "Accept", () => accept = true },
                                                { "Reject", () => accept = false }
                                            });
                                            if (!accept)
                                                return;
                                            while (true)
                                            {
                                                if (!secureStream.TryReceiveDatagram(TimeSpan.FromMinutes(1), out var datagram))
                                                {
                                                    "Failed to get datagram. Trying again...".Say();
                                                    continue;
                                                }
                                                var message = Encoding.ASCII.GetString(datagram);
                                                message.Say();
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            });
        }

        #endregion
    }
}

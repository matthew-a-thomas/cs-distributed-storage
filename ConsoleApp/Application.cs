namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking;
    using DistributedStorage.Networking.Protocol;
    using DistributedStorage.Networking.Security;
    using DistributedStorage.Solving;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.FileSystem;
    using Newtonsoft.Json;
    using Directory = System.IO.Directory;
    using File = System.IO.File;

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
        private readonly IGeneratorFactory _generatorFactory;
        private readonly IManifestFactory _manifestFactory;
        private readonly ISolverFactory _solverFactory;
        private readonly StorageFactory _storageFactory;
        private readonly DatagramProtocol.Factory _datagramProtocolFactory;
        private readonly Node.Factory _nodeFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="Application"/>
        /// </summary>
        public Application(
            HashVisualizer hashVisualizer,
            SecureStreamFactory secureStreamFactory,
            ICryptoRsa cryptoRsa,
            IGeneratorFactory generatorFactory,
            IManifestFactory manifestFactory,
            ISolverFactory solverFactory,
            StorageFactory storageFactory,
            DatagramProtocol.Factory datagramProtocolFactory,
            Node.Factory nodeFactory)
        {
            _hashVisualizer = hashVisualizer;
            _secureStreamFactory = secureStreamFactory;
            _cryptoRsa = cryptoRsa;
            _generatorFactory = generatorFactory;
            _manifestFactory = manifestFactory;
            _solverFactory = solverFactory;
            _storageFactory = storageFactory;
            _datagramProtocolFactory = datagramProtocolFactory;
            _nodeFactory = nodeFactory;
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
        private void GenerateSlicesFromFile()
        {
            $"You are currently in {Directory.GetCurrentDirectory()}".Say();

            var filename = "File name?".Ask();
            "Reading...".Say();
            var data = File.ReadAllBytes(filename);
            $"Read {data.Length} bytes".Say();

            if (!int.TryParse("How many chunks?".Ask(), out var numChunks))
                numChunks = 10;
            $"Will use {numChunks} chunks".Say();
            
            "Generating manifest...".Say();
            var manifest = _manifestFactory.CreateManifestFrom(data, numChunks);
            var manifestOutputFileName = $"{filename}.manifest";
            using (var manifestOutputFile = File.OpenWrite(manifestOutputFileName))
            {
                manifestOutputFile.Write(manifest);
            }
            manifestOutputFileName.Say();

            "Splitting data into chunks...".Say();
            var parts = data.SplitInto(numChunks);

            "Creating slice generator...".Say();
            var generator = _generatorFactory.CreateGeneratorFor(parts);

            for (var i = 0; ; ++i)
            {
                "Press any key to generate a slice...".Wait();

                var slice = generator.Next();
                var outputFilename = $"{filename}.{i}";
                using (var stream = File.OpenWrite(outputFilename))
                    stream.Write(slice);

                outputFilename.Say();
            }
        }

        /// <summary>
        /// Guides the user through a simulation of sending slices over a lossy communications channel
        /// </summary>
        private void PlayWithSlices()
        {
            const int numSlices = 5;
            var data = Encoding.ASCII.GetBytes("Hello world!");
            var manifest = _manifestFactory.CreateManifestFrom(data, numSlices);
            var parts = data.SplitInto(numSlices);
            var generator = _generatorFactory.CreateGeneratorFor(parts);
            var solver = _solverFactory.CreateSolverFor(manifest);

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

        /// <summary>.com
        /// Guides the user through recreating a file from slices
        /// </summary>
        private void ReadSlices()
        {
            $"You are currently in {Directory.GetCurrentDirectory()}".Say();
            var manifestFileInfo = new FileInfo("Manifest?".Ask());
            Manifest manifest;
            using (var stream = manifestFileInfo.OpenRead())
            {
                if (!stream.TryRead(out manifest))
                    throw new Exception("Couldn't read manifest");
            }
            
            var solver = _solverFactory.CreateSolverFor(manifest);
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
                    if (!SliceExtensions.TryRead(stream, out var slice))
                        throw new Exception("Couldn't read slice");

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
                        var storage = _storageFactory.CreateStorage(new DirectoryInfo("Working directory?".Ask()).ToDirectory());

                        var key = default(RSAParameters);
                        {
                            var create = true;
                            if (storage.OurRsaKeyFile.TryOpenRead(out Stream stream))
                            {
                                using (stream)
                                {
                                    if (stream.TryRead(out key))
                                        create = false;
                                }
                            }

                            if (create)
                            {
                                if (storage.OurRsaKeyFile.TryOpenWrite(out stream))
                                {
                                    using (stream)
                                    {
                                        "Generating an RSA key...".Say();
                                        key = _cryptoRsa.CreateKey();
                                        stream.Write(key);
                                    }
                                }
                                else
                                {
                                    throw new Exception("RSA key could neither be read nor written");
                                }
                            }

                            $"Your RSA key has this fingerprint: {key.ToHash().HashCode.ToHex()}".Say();
                        }

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
                                            if (!_secureStreamFactory.TryCreateConnection(stream, key, SecureStreamFactory.Mode.Make, out var theirs, out var secureStream))
                                            {
                                                "Failed to connect".Say();
                                                return;
                                            }
                                            var accept = false;
                                            theirs.ToHash().HashCode.ToHex().Choose(new Dictionary<string, Action>
                                            {
                                                { "Accept", () => accept = true },
                                                { "Reject", () => accept = false }
                                            });
                                            if (!accept)
                                                return;

                                            var protocol = _datagramProtocolFactory.Create(secureStream);
                                            if (!_nodeFactory.TryCreate(storage, protocol, out var node))
                                                throw new Exception("Failed to create a new node");
                                            using (node)
                                            while (true)
                                            {
                                                "Do what?".Choose(new Dictionary<string, Action>
                                                {
                                                    {
                                                        "List manifests",
                                                        () => node.GetManifestsAsync().ContinueWith(task =>
                                                        {
                                                            if (!task.IsCompleted || task.IsCanceled || task.IsFaulted)
                                                                return;
                                                            var manifests = task.Result;
                                                            "<manifests>".Say();
                                                            string.Join(Environment.NewLine, manifests.Select(manifest => manifest.Id.HashCode.ToHex())).Say();
                                                            "</manifests>".Say();
                                                        })
                                                    },
                                                    {
                                                        "Pump message queue",
                                                        protocol.Pump
                                                    }
                                                });
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
                                            if (!_secureStreamFactory.TryCreateConnection(stream, key, SecureStreamFactory.Mode.Accept, out var theirs, out var secureStream))
                                            {
                                                "Failed to accept a connection".Say();
                                                return;
                                            }
                                            var accept = false;
                                            theirs.ToHash().HashCode.ToHex().Choose(new Dictionary<string, Action>
                                            {
                                                { "Accept", () => accept = true },
                                                { "Reject", () => accept = false }
                                            });
                                            if (!accept)
                                                return;

                                            var protocol = _datagramProtocolFactory.Create(secureStream);
                                            if (!_nodeFactory.TryCreate(storage, protocol, out var node))
                                                throw new Exception("Failed to create a new node");
                                            using (node)
                                            while (true)
                                            {
                                                protocol.Pump();
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

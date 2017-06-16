namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using DistributedStorage.Actors;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking;
    using DistributedStorage.Networking.Protocol;
    using DistributedStorage.Networking.Security;
    using DistributedStorage.Solving;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.Containers;
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
        private readonly IEntropy _entropy;

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
            Node.Factory nodeFactory,
            IEntropy entropy)
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
            _entropy = entropy;
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

                        // Set up a dummy manifest container
                        storage.ContainersForManifests.GetOrCreate(_manifestFactory.CreateManifestFrom(Encoding.ASCII.GetBytes("Hello world. This is the data for a dummy manifest")));

                        var key = storage.GetOrCreateOurRsaKey(() =>
                        {
                            "A new RSA key is being generated. One moment...".Say();
                            return _cryptoRsa.CreateKey();
                        });

                        "Your RSA key has this fingerprint:".Say();
                         key.ToHash().HashCode.ToHex().Say();

                        TcpClient client;
                        var endpoint = new IPEndPoint(IPAddress.Loopback, 1337);
                        try
                        {
                            var listener = new TcpListener(endpoint);
                            listener.Start();
                            $"Waiting for anyone to connect to {endpoint}...".Say();
                            client = listener.AcceptTcpClientAsync().WaitAndGet();
                            $"A client connected from {client.Client.RemoteEndPoint}".Say();
                        }
                        catch
                        {
                            client = new TcpClient();
                            $"Connecting to {endpoint}...".Say();
                            client.ConnectAsync(endpoint.Address, endpoint.Port).Wait();
                            "Connected".Say();
                        }
                        using (client)
                        using (var stream = client.GetStream())
                        {
                            // Figure out what mode we should be in with regard to creating a SecureStream
                            "Figuring out who should have what role with regard to creating a new secure stream...".Say();
                            SecureStreamFactory.Mode mode;
                            // ReSharper disable AccessToDisposedClosure
                            var tieBreaker = new TieBreaker(_entropy, ourNumber => stream.Write(ourNumber), () => stream.TryRead(out int theirNumber) ? theirNumber : throw new Exception("They didn't send a number"));
                            // ReSharper restore AccessToDisposedClosure
                            switch (tieBreaker.Test())
                            {
                                case TieBreaker.Result.TheyWon:
                                    mode = SecureStreamFactory.Mode.Accept;
                                    "They won the tie break".Say();
                                    break;
                                case TieBreaker.Result.Tie:
                                    throw new Exception("No one won the tie break");
                                case TieBreaker.Result.YouWon:
                                    mode = SecureStreamFactory.Mode.Make;
                                    "We won the tie break".Say();
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                            // Create a SecureStream
                            "Creating a secure stream...".Say();
                            if (!_secureStreamFactory.TryCreateConnection(stream, key, mode, out var theirs, out var secureStream))
                                throw new Exception("Failed to create a secure stream");
                            var theirKeyHash = theirs.ToHash();
                            "This is their public key:".Say();
                            theirKeyHash.HashCode.ToHex().Say();

                            // Deal with their public key
                            if (storage.TrustedPublicKeys.ContainsKey(theirKeyHash))
                            {
                                var useIt = false;
                                "We already trust their key".Choose(new Dictionary<string, Action>
                                {
                                    {
                                        "Use it",
                                        () => useIt = true
                                    },
                                    {
                                        "Stop trusting it",
                                        () => useIt = false
                                    }
                                });

                                if (!useIt)
                                {
                                    storage.TrustedPublicKeys.TryRemove(theirKeyHash);

                                    "Goodbye".Say();
                                    return;
                                }
                            }
                            else
                            {
                                var accept = false;
                                "They have an unknown key".Choose(new Dictionary<string, Action>
                                {
                                    {
                                        "Trust it",
                                        () => accept = true
                                    },
                                    {
                                        "Reject it",
                                        () => accept = false
                                    }
                                });

                                if (!accept)
                                {
                                    "Goodbye".Say();
                                    return;
                                }

                                storage.TrustedPublicKeys.TryAdd(theirKeyHash, theirs);
                            }

                            // Set up the protocol
                            "Setting up a datagram protocol...".Say();
                            var handlerDispatcher = Dispatcher.Create(action => Task.Run(action));
                            var protocol = _datagramProtocolFactory.Create(secureStream, handlerDispatcher, handlerDispatcher);

                            // Set up the corresponding node, which connects the protocol to our storage
                            "Setting up a node, which connects the protocol to our storage...".Say();
                            if (!_nodeFactory.TryCreate(storage, protocol, out var node))
                                throw new Exception("Failed to create a new node to connect our storage with the communication protocol");

                            // Begin pumping the protocol
                            "Starting to pump the protocol...".Say();
                            var datagramDispatcher = Dispatcher.Create(action => Task.Run(action));
                            void PumpProtocol()
                            {
                                protocol.Pump();
                                "Received a protocol message...".Say();
                                datagramDispatcher.BeginInvoke(PumpProtocol);
                            }
                            datagramDispatcher.BeginInvoke(PumpProtocol);

                            // Let the user drive
                            var go = true;
                            while (go)
                            {
                                "Do what?".Choose(new Dictionary<string, Action>
                                {
                                    {
                                        "Quit",
                                        () => go = false
                                    },
                                    {
                                        "List their manifests",
                                        () =>
                                            node
                                            .GetManifestsAsync()
                                            .DoAfterSuccess(manifests =>
                                            {
                                                "<manifests>".Say();
                                                string.Join(Environment.NewLine, manifests.Select(manifest => manifest.Id.HashCode.ToHex())).Say();
                                                "</manifests>".Say();
                                            })
                                    }
                                });
                            }
                        }
                    }
                }
            });
        }

        #endregion
    }
}

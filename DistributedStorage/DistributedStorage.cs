namespace DistributedStorage
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Autofac;

    /// <summary>
    /// A facade for internal implementations of <see cref="IManifestFactory"/>, <see cref="IGeneratorFactory"/>, and <see cref="ISolverFactory"/>
    /// </summary>
    public sealed class DistributedStorage : IDisposable, IManifestFactory, IGeneratorFactory, ISolverFactory
    {
        /// <summary>
        /// The thing to dispose when we're through
        /// </summary>
        private IDisposable _disposable;

        /// <summary>
        /// Our internal implementation of <see cref="IGeneratorFactory"/>
        /// </summary>
        private IGeneratorFactory _generatorFactory;

        /// <summary>
        /// Our internal implementation of <see cref="IManifestFactory"/>
        /// </summary>
        private IManifestFactory _manifestFactory;

        /// <summary>
        /// Our internal implementation of <see cref="ISolverFactory"/>
        /// </summary>
        private ISolverFactory _solverFactory;

        /// <summary>
        /// Creates a new facade for internal implementations of <see cref="IManifestFactory"/>, <see cref="IGeneratorFactory"/>, and <see cref="ISolverFactory"/>
        /// </summary>
        public DistributedStorage()
        {
            // Register
            var builder = new ContainerBuilder();
            builder.RegisterModule<Module>();
            var container = builder.Build();
            _disposable = container;

            // Resolve
            _generatorFactory = container.Resolve<IGeneratorFactory>();
            _manifestFactory = container.Resolve<IManifestFactory>();
            _solverFactory = container.Resolve<ISolverFactory>();
        }
     
        /// <summary>
        /// Creates a new <see cref="Manifest"/> for the given data
        /// </summary>
        public Manifest CreateManifestFrom(byte[] data, int numSlices = 10) => _manifestFactory.CreateManifestFrom(data, numSlices);

        /// <summary>
        /// Creates an <see cref="IGenerator"/> for the given data slices
        /// </summary>
        public IGenerator CreateGeneratorFor(IReadOnlyList<byte[]> source) => _generatorFactory.CreateGeneratorFor(source);

        /// <summary>
        /// Creates a new <see cref="ISolver"/> which can solve for the data which produced the given <paramref name="manifest"/>
        /// </summary>
        public ISolver CreateSolverFor(Manifest manifest) => _solverFactory.CreateSolverFor(manifest);

        /// <summary>
        /// Releases all internal resources
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref _disposable, null);
            if (disposable == null)
                return;
            _generatorFactory = null;
            _manifestFactory = null;
            _solverFactory = null;
            disposable.Dispose();
        }
    }
}

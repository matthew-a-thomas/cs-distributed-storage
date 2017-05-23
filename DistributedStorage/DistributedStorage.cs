namespace DistributedStorage
{
    using System;
    using System.Threading;
    using Autofac;

    /// <summary>
    /// A facade for internal implementations of <see cref="IManifestFactory"/>, <see cref="IGeneratorFactory"/>, and <see cref="ISolverFactory"/>
    /// </summary>
    public sealed class DistributedStorage : IDisposable
    {
        /// <summary>
        /// The thing to dispose when we're through
        /// </summary>
        private IDisposable _disposable;

        /// <summary>
        /// Our internal implementation of <see cref="IGeneratorFactory"/>
        /// </summary>
        public IGeneratorFactory GeneratorFactory { get; private set; }

        /// <summary>
        /// Our internal implementation of <see cref="IManifestFactory"/>
        /// </summary>
        public IManifestFactory ManifestFactory { get; private set; }

        /// <summary>
        /// Our internal implementation of <see cref="ISolverFactory"/>
        /// </summary>
        public ISolverFactory SolverFactory { get; private set; }

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
            GeneratorFactory = container.Resolve<IGeneratorFactory>();
            ManifestFactory = container.Resolve<IManifestFactory>();
            SolverFactory = container.Resolve<ISolverFactory>();
        }
     
        /// <summary>
        /// Releases all internal resources
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref _disposable, null);
            if (disposable == null)
                return;
            GeneratorFactory = null;
            ManifestFactory = null;
            SolverFactory = null;
            disposable.Dispose();
        }
    }
}

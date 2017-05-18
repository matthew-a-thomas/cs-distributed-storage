namespace DistributedStorage
{
    internal class SolverFactory : ISolverFactory
    {
        private readonly IManifestFactory _manifestFactory;

        public SolverFactory(IManifestFactory manifestFactory)
        {
            _manifestFactory = manifestFactory;
        }

        public ISolver CreateSolverFor(Manifest manifest) => new Solver(manifest, _manifestFactory);
    }
}

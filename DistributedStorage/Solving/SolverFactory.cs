﻿namespace DistributedStorage.Solving
{
    using Encoding;

    public sealed class SolverFactory : ISolverFactory
    {
        private readonly IManifestFactory _manifestFactory;

        public SolverFactory(IManifestFactory manifestFactory)
        {
            _manifestFactory = manifestFactory;
        }

        public ISolver CreateSolverFor(Manifest manifest) => new Solver(manifest, _manifestFactory);
    }
}

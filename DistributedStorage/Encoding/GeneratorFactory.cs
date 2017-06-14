namespace DistributedStorage.Encoding
{
    using System;
    using System.Collections.Generic;
    using Networking.Security;

    public sealed class GeneratorFactory : IGeneratorFactory
    {
        private readonly Func<IEntropy> _randomFactory;

        public GeneratorFactory(Func<IEntropy> randomFactory)
        {
            _randomFactory = randomFactory;
        }

        public IGenerator CreateGeneratorFor(IReadOnlyList<byte[]> source)
        {
            var entropy = _randomFactory();
            return new Generator(entropy, source);
        }
    }
}

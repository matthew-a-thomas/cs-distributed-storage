namespace DistributedStorage.Encoding
{
    using System;
    using System.Collections.Generic;
    using Common;

    public sealed class GeneratorFactory : IGeneratorFactory
    {
        private readonly Func<IRandom> _randomFactory;

        public GeneratorFactory(Func<IRandom> randomFactory)
        {
            _randomFactory = randomFactory;
        }

        public IGenerator CreateGeneratorFor(IReadOnlyList<byte[]> source)
        {
            var random = _randomFactory();
            return new Generator(random, source);
        }
    }
}

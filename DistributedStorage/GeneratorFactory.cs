namespace DistributedStorage
{
    using System;
    using System.Collections.Generic;

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

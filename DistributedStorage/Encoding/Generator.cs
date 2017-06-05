namespace DistributedStorage.Encoding
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Generates <see cref="Slice"/>s from source data
    /// </summary>
    internal class Generator : IGenerator
    {
        /// <summary>
        /// The number of times <see cref="Next"/> has been called
        /// </summary>
        private int _numGenerated;

        /// <summary>
        /// Our source of entropy
        /// </summary>
        private readonly IRandom _random;

        /// <summary>
        /// The source data
        /// </summary>
        private readonly IReadOnlyList<byte[]> _source;

        /// <summary>
        /// Creates a new <see cref="Generator"/> that pulls entropy from <paramref name="random"/> and generates encoding symbols from <paramref name="source"/>
        /// </summary>
        public Generator(IRandom random, IReadOnlyList<byte[]> source)
        {
            _random = random;
            _source = source;
        }

        /// <summary>
        /// Generates a new <see cref="Slice"/>
        /// </summary>
        public Slice Next()
        {
            var coefficients = new bool[_source.Count]; // This will track which symbols were XOR'd together
            var encodingSymbol = new byte[_source[0].Length]; // This will contain the encoding symbol's contents

            if (_numGenerated < _source.Count)
            {
                // Start with a systematic code, which just outputs the source symbols in order before it begins combining them together
                coefficients[_numGenerated] = true;
                _source[_numGenerated].CopyTo(encodingSymbol, 0);
                ++_numGenerated;
            }
            else
            {
                // XOR together a random subset of slices from the source data
                var numPickedCoefficients = 0; // Keeps track of how many source symbols have been combined so far
                while (numPickedCoefficients == 0) // Make sure we include at least one of the source symbols
                {
                    // XOR a random subset of the source symbols into the encoding symbol
                    for (var i = 0; i < coefficients.Length; ++i)
                    {
                        if (coefficients[i])
                            continue;
                        var bit = _random.NextBit();
                        if (!bit)
                            continue;
                        ++numPickedCoefficients;
                        coefficients[i] = true;
                        encodingSymbol.Xor(_source[i]);
                    }
                }
            }

            return new Slice
            {
                Coefficients = coefficients,
                EncodingSymbol = encodingSymbol
            };
        }
    }
}

namespace DistributedStorage
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Creates <see cref="Manifest"/>s from data
    /// </summary>
    internal class ManifestFactory : IManifestFactory
    {
        /// <summary>
        /// Something that can create new instances of <see cref="HashAlgorithm"/> for us to use
        /// </summary>
        private readonly Func<HashAlgorithm> _hasherFactory;

        /// <summary>
        /// Creates a new <see cref="ManifestFactory"/>, which creates instances of <see cref="Manifest"/> by using the given <paramref name="hasherFactory"/>
        /// </summary>
        public ManifestFactory(Func<HashAlgorithm> hasherFactory)
        {
            _hasherFactory = hasherFactory;
        }

        /// <summary>
        /// Creates a new <see cref="Manifest"/> from the given <paramref name="data"/>, using the given <paramref name="numSlices"/>
        /// </summary>
        public Manifest CreateManifestFrom(byte[] data, int numSlices = 10)
        {
            if (data.Length < numSlices)
                throw new ArgumentException($"Cannot split {data.Length} bytes up into {numSlices} slices. Choose a lower number of slices");

            using (var hasher = _hasherFactory())
            {
                var wholeHash = new Hash(hasher.ComputeHash(data));
                var sliceHashes = new Hash[numSlices];
                var sliceSize = (int)Math.Ceiling(data.Length / (double)numSlices);
                for (var slice = 0; slice < numSlices; ++slice)
                {
                    var startingIndex = slice * sliceSize;
                    var length = Math.Min(sliceSize, data.Length - startingIndex);
                    var sliceHash = new Hash(hasher.ComputeHash(data, startingIndex, length));
                    sliceHashes[slice] = sliceHash;
                }
                return new Manifest
                {
                    Id = wholeHash,
                    Length = data.Length,
                    SliceHashes = sliceHashes
                };
            }
        }
    }
}

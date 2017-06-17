namespace DistributedStorage.Encoding
{
    using System;
    using Common;

    /// <summary>
    /// Creates <see cref="Manifest"/>s from data
    /// </summary>
    public sealed class ManifestFactory : IManifestFactory
    {
        /// <summary>
        /// Creates a new <see cref="Manifest"/> from the given <paramref name="data"/>, using the given <paramref name="numSlices"/>
        /// </summary>
        public Manifest CreateManifestFrom(byte[] data, int numSlices = 10)
        {
            if (data.Length < numSlices)
                throw new ArgumentException($"Cannot split {data.Length} bytes up into {numSlices} slices. Choose a lower number of slices");

            var wholeHash = Hash.Create(data);
            var sliceHashes = new Hash[numSlices];
            var sliceSize = (int)Math.Ceiling(data.Length / (double)numSlices);
            for (var slice = 0; slice < numSlices; ++slice)
            {
                var startingIndex = slice * sliceSize;
                var length = Math.Min(sliceSize, data.Length - startingIndex);
                var sliceData = new byte[length];
                Array.Copy(data, startingIndex, sliceData, 0, length);
                var sliceHash = Hash.Create(sliceData);
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

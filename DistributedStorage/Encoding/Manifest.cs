namespace DistributedStorage.Encoding
{
    using Common;

    /// <summary>
    /// Metadata about a distributed document
    /// </summary>
    public class Manifest
    {
        /// <summary>
        /// The hash code of the entire document's contents
        /// </summary>
        public Hash Id { get; set; }

        /// <summary>
        /// The number of bytes in the document
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The hash codes of the individual slices
        /// </summary>
        public Hash[] SliceHashes { get; set; }

        /// <summary>
        /// Returns a string representation of this <see cref="Manifest"/>
        /// </summary>
        public override string ToString() => Id.ToString();
    }
}

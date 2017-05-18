namespace DistributedStorage
{
    /// <summary>
    /// Contains space for a 256-bit hash code
    /// </summary>
    public class Hash
    {
        /// <summary>
        /// The number of bytes to store
        /// </summary>
        private const int NumBytes = 256 / 8;

        /// <summary>
        /// The contained hash code.
        /// This is guaranteed to be 32 bytes long
        /// </summary>
        public readonly byte[] HashCode;

        /// <summary>
        /// Creates a new empty hash code containing 32 bytes
        /// </summary>
        public Hash()
        {
            HashCode = new byte[NumBytes];
        }

        /// <summary>
        /// Creates a new hash code from the given <paramref name="hashCode"/>,
        /// which must be at most 32 bytes long
        /// </summary>
        public Hash(byte[] hashCode)
            : this()
        {
            hashCode.CopyTo(HashCode, 0);
        }
    }
}

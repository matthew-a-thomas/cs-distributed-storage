namespace DistributedStorage.Common
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Contains space for a 256-bit hash code
    /// </summary>
    public class Hash : IComparable<Hash>, IComparable, IEquatable<Hash>
    {
        /// <summary>
        /// The number of bytes to store
        /// </summary>
        public const int NumBytes = 256 / 8;

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

        #region IComparable

        public int CompareTo(Hash other) => FindDifference(this, other);

        int IComparable.CompareTo(object other) => CompareTo(other as Hash);

        #endregion

        #region IEquatable

        /// <summary>
        /// Determines if this <see cref="Hash"/> matches the <paramref name="other"/> <see cref="Hash"/>
        /// </summary>
        public bool Equals(Hash other) => !ReferenceEquals(other, null) && FindDifference(this, other) == 0;

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns true if the given <paramref name="obj"/> is the same as this, or is a <see cref="Hash"/> with the same <see cref="HashCode"/> contents
        /// </summary>
        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Hash other && FindDifference(this, other) == 0;

        /// <summary>
        /// Returns a combination of <see cref="HashCode"/>'s bytes
        /// </summary>
        public override int GetHashCode()
        {
            var combined = new byte[4];
            for (var i = 0; i < NumBytes; ++i)
            {
                combined[i % 4] ^= HashCode[i];
            }
            return BitConverter.ToInt32(combined, 0);
        }
        
        #endregion

        #region Static methods

        /// <summary>
        /// Creates a new <see cref="Hash"/> by performing SHA256 on the given byte array
        /// </summary>
        public static Hash Create(byte[] from)
        {
            using (var hasher = SHA256.Create())
                return new Hash(hasher.ComputeHash(from));
        }

        /// <summary>
        /// Returns 1 if the first byte that is different is from <paramref name="a"/>, -1 if from <paramref name="b"/>, or 0 if all their bytes are the same
        /// </summary>
        private static int FindDifference(Hash a, Hash b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return 0;
            if (ReferenceEquals(a, null))
                return -1;
            if (ReferenceEquals(b, null))
                return 1;
            for (var i = 0; i < NumBytes; ++i)
            {
                if (a.HashCode[i] != b.HashCode[i])
                    return a.HashCode[i] > b.HashCode[i] ? 1 : -1;
            }
            return 0;
        }

        #endregion
    }
}

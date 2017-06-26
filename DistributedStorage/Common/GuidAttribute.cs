namespace DistributedStorage.Common
{
    using System;

    /// <summary>
    /// Puts a GUID on something
    /// </summary>
    public sealed class GuidAttribute : Attribute
    {
        /// <summary>
        /// The GUID that was put on it
        /// </summary>
        public Guid Guid { get; }

        public GuidAttribute(string guid) => Guid = Guid.Parse(guid);
    }
}

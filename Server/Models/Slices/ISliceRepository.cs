namespace Server.Models.Slices
{
    using System.Collections.Generic;
    using DistributedStorage.Encoding;

    /// <summary>
    /// A repository for <see cref="Slice"/>s
    /// </summary>
    public interface ISliceRepository
    {
        /// <summary>
        /// Lists all currently available <see cref="Slice"/> IDs
        /// </summary>
        IEnumerable<string> ListSliceIds();

        /// <summary>
        /// Tries to add the given <paramref name="slice"/>
        /// </summary>
        bool TryAddSlice(Slice slice);

        /// <summary>
        /// Tries to delete the <see cref="Slice"/> having the given <paramref name="id"/>
        /// </summary>
        bool TryDeleteSliceWithId(string id);

        /// <summary>
        /// Tries to get the <see cref="Slice"/> having the given <paramref name="id"/>
        /// </summary>
        bool TryGetSliceWithId(string id, out Slice slice);
    }
}
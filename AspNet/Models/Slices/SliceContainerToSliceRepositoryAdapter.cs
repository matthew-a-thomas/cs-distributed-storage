namespace AspNet.Models.Slices
{
    using System.Collections.Generic;
    using System.Linq;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Storage.Containers;

    /// <summary>
    /// An implementation of <see cref="ISliceRepository"/> that adapts an <see cref="IAddableContainer{TKey, TValue}"/>
    /// </summary>
    public class SliceContainerToSliceRepositoryAdapter : ISliceRepository
    {
        private readonly IAddableContainer<Hash, Slice> _sliceContainer;

        public SliceContainerToSliceRepositoryAdapter(IAddableContainer<Hash, Slice> sliceContainer) => _sliceContainer = sliceContainer;
        
        public IEnumerable<string> ListSliceIds() => _sliceContainer.GetKeys().Select(hash => hash.ToString());
        public bool TryAddSlice(Slice slice) => _sliceContainer.TryAdd(slice.ComputeHash(), slice);
        public bool TryDeleteSliceWithId(string id) => _sliceContainer.TryRemove(new Hash(id.ToBytes()));
        public bool TryGetSliceWithId(string id, out Slice slice) => _sliceContainer.TryGet(new Hash(id.ToBytes()), out slice);
    }
}

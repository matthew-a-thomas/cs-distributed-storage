namespace DistributedStorage.Storage
{
    using System.Collections.Generic;
    using Encoding;

    public interface ISliceContainer
    {
        void AddSlice(Slice slice);

        IEnumerable<Slice> GetSlices();
    }
}

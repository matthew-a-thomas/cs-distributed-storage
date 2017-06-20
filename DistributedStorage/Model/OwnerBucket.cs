namespace DistributedStorage.Model
{
    using System;

    public sealed class OwnerBucket : IOwnerBucket
    {
        private readonly Action _delete;

        public OwnerBucket(Action delete)
        {
            _delete = delete;
        }

        public void Delete() => _delete();
    }
}

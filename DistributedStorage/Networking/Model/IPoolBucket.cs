namespace DistributedStorage.Networking.Model
{
    using System.Collections.Generic;
    using Encoding;

    /// <summary>
    /// A bucket from the perspective of the <see cref="IPool"/> that is authorized to manage it
    /// </summary>
    public interface IPoolBucket
    {
        IEnumerable<Manifest> GetManifests();
    }
}

namespace DistributedStorage.Encoding
{
    using Common;

    /// <summary>
    /// Creates <see cref="Manifest"/>s from data
    /// </summary>
    public interface IManifestFactory
    {
        /// <summary>
        /// Creates a new <see cref="Manifest"/> from the given <paramref name="data"/>, using the given <paramref name="numSlices"/>
        /// </summary>
        Manifest CreateManifestFrom(byte[] data, int numSlices = 10);
    }
}

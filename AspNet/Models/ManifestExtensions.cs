namespace AspNet.Models
{
    using System.Diagnostics.CodeAnalysis;

    internal static class ManifestExtensions
    {
        [SuppressMessage("ReSharper", "ArgumentsStyleOther")]
        public static Manifest ToManifest(this DistributedStorage.Encoding.Manifest manifest) => new Manifest(
            id: manifest.Id.ToString(),
            length: manifest.Length,
            numSlices: manifest.SliceHashes.Length
        );
    }
}

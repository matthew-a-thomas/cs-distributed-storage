namespace DistributedStorage.Solving
{
    using Common;

    /// <summary>
    /// Solves for data
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// Tries to recover the original data for the <see cref="Manifest"/> for the data which produced this <paramref name="slice"/> and all past given slices.
        /// If successful, true is returned and <paramref name="solution"/> will contain the original data.
        /// If the internal solver was able to reconstruct the data but the <see cref="Manifest"/> doesn't match, then false is returned and <paramref name="solution"/> will contain that data.
        /// Otherwise, false is returned and <paramref name="solution"/> is set to null
        /// </summary>
        bool TrySolve(Slice slice, out byte[] solution);
    }
}

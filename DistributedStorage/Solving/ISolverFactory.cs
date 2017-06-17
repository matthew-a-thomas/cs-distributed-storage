namespace DistributedStorage.Solving
{
    using Encoding;

    public interface ISolverFactory
    {
        ISolver CreateSolverFor(Manifest manifest);
    }
}

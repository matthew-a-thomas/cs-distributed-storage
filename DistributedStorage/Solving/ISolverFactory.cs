namespace DistributedStorage.Solving
{
    using Common;

    public interface ISolverFactory
    {
        ISolver CreateSolverFor(Manifest manifest);
    }
}

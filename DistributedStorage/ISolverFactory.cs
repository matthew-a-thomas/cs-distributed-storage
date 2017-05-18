namespace DistributedStorage
{
    public interface ISolverFactory
    {
        ISolver CreateSolverFor(Manifest manifest);
    }
}

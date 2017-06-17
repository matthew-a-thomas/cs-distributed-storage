namespace DistributedStorage.Networking.Protocol
{
    public interface IHandler<in TParameter, out TResult>
    {
        TResult Handle(TParameter parameter);
    }
}

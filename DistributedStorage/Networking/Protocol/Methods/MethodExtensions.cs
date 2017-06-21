namespace DistributedStorage.Networking.Protocol.Methods
{
    using System.Threading.Tasks;
    using Common;

    public static class MethodExtensions
    {
        public static TResult InvokeAndWait<TParameter, TResult>(this IMethod<TParameter, TResult> method, TParameter parameter)
        {
            var tcs = new TaskCompletionSource<TResult>();
            method.Invoke(parameter, tcs.SetResult);
            return tcs.Task.WaitAndGet();
        }
    }
}

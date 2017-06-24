namespace DistributedStorage.Networking.Protocol.Methods
{
    using System.Threading.Tasks;

    public static class MethodExtensions
    {
        /// <summary>
        /// Invokes this <see cref="IMethod{TParameter, TResult}"/>, treating it like an async function
        /// </summary>
        public static Task<TResult> InvokeAsync<TParameter, TResult>(this IMethod<TParameter, TResult> method, TParameter parameter)
        {
            var tcs = new TaskCompletionSource<TResult>();
            method.Invoke(parameter, tcs.SetResult);
            return tcs.Task;
        }
    }
}

namespace DistributedStorage.Networking.Protocol.Methods
{
    using System;

    public interface IMethod<in TParameter, out TResult> : IDisposable
    {
        void Invoke(TParameter parameter, Action<TResult> callback);
    }
}
namespace DistributedStorage.Actors
{
    using System;

    public interface IDispatcher
    {
        void BeginInvoke(Action action);
    }
}
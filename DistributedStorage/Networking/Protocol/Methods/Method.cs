namespace DistributedStorage.Networking.Protocol.Methods
{
    using System;

    public sealed class Method<TParameter, TResult> : IMethod<TParameter, TResult>
    {
        public delegate void InvokeDelegate(TParameter parameter, Action<TResult> callback);

        public sealed class Options
        {
            public Action Dispose { get; set; } = () => { };
            public InvokeDelegate Invoke { get; set; } = (parameter, callback) => { };
        }

        private readonly Options _options;

        public Method(Options options = null)
        {
            _options = options ?? new Options();
        }

        public void Invoke(TParameter parameter, Action<TResult> callback) => _options.Invoke(parameter, callback);

        public void Dispose() => _options.Dispose();
    }
}

namespace DistributedStorage.Networking.Protocol.Methods
{
    public interface IProtocolMethodFactory<TParameter, TResult>
    {
        /// <summary>
        /// Tries to create a new <see cref="IMethod{TParameter, TResult}"/> that will operate on the given <paramref name="protocol"/>
        /// </summary>
        /// <param name="protocol">The protocol through which the returned <paramref name="method"/> will be invoked</param>
        /// <param name="signature">The signature of the <paramref name="method"/> that is being created</param>
        /// <param name="handler">Something that handles requests to the created <paramref name="method"/></param>
        /// <param name="method">The created <see cref="IMethod{TParameter, TResult}"/></param>
        bool TryCreate(IProtocol protocol, string signature, IHandler<TParameter, TResult> handler, out IMethod<TParameter, TResult> method);
    }
}
namespace DistributedStorage.Networking.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Common;

    /// <summary>
    /// An <see cref="IProtocolInitializer{T}"/> that registers <see cref="IHandler{TParameter, TResult}"/> factories for strings
    /// </summary>
    public sealed class ProtocolInitializer<T> : IProtocolInitializer<T>
    {
        /// <summary>
        /// The list of handler factories to use
        /// </summary>
        private readonly IReadOnlyList<Tuple<string, Func<T, IHandler<byte[], byte[]>>>> _handlerFactoryTuples;

        /// <summary>
        /// Creates a new <see cref="ProtocolInitializer{T}"/>, which registers <see cref="IHandler{TParameter, TResult}"/> factories for strings
        /// </summary>
        public ProtocolInitializer(IReadOnlyList<Tuple<string, Func<T, IHandler<byte[], byte[]>>>> handlerFactoryTuples)
        {
            _handlerFactoryTuples = handlerFactoryTuples;
        }
        
        /// <summary>
        /// Tries to register an <see cref="IHandler{TParameter, TResult}"/> for each string in the list that was passed into this <see cref="ProtocolInitializer{T}"/>'s constructor
        /// </summary>
        public bool TrySetup(IProtocol protocol, T with, out IDisposable tearDown)
        {
            // Start a list of registered strings
            var registeredStrings = new List<string>();

            // Set up the tear down routine, which will try to unregister all registered strings
            tearDown = new Disposable(() =>
            {
                foreach (var registeredString in registeredStrings)
                    protocol.TryUnregister(registeredString);
            });

            // Run through each handler factory, and register each generated handler with the corresponding string
            foreach (var tuple in _handlerFactoryTuples)
            {
                var s = tuple.Item1;
                var handlerFactory = tuple.Item2;
                var handler = handlerFactory(with);
                if (!protocol.TryRegister(s, handler))
                    return false;
                registeredStrings.Add(s); // We successfully registered this string with the protocol, so add it to the list of registered strings
            }

            return true;
        }
    }

    public static class ProtocolInitializer
    {
        /// <summary>
        /// Creates the handler factory tuples list which can be used to create a new <see cref="ProtocolInitializer{T}"/> for the given type <typeparamref name="T"/>, using the given <paramref name="deserializerLookup"/> and <paramref name="serializerLookup"/> to create <see cref="IHandler{TParameter, TResult}"/>s for all methods in the type <typeparamref name="T"/>
        /// </summary>
        public static bool TryCreate<T>(Func<Type, IConverter<byte[], object>> deserializerLookup, Func<Type, IConverter<object, byte[]>> serializerLookup, out ProtocolInitializer<T> initializer)
        {
            try
            {
                var myHandlerFactoryTuples = new List<Tuple<string, Func<T, IHandler<byte[], byte[]>>>>();

                // Loop through all methods in type T
                foreach (var method in typeof(T).GetMethods())
                {
                    // Grab a string that uniquely identifies this method
                    var name = method.GetStrongName();
                    name = Hash.Create(System.Text.Encoding.UTF8.GetBytes(name)).HashCode.ToHex();
                    
                    // Set up a handler factory for this method and a given object of the type T
                    IHandler<byte[], byte[]> CreateHandlerFor(T obj)
                    {
                        // Create an IHandler that will take a byte array as input,
                        // transform it into the series of parameters required for this method,
                        // pass those parameters through this method (on the given object "obj"),
                        // then serialize the return value into a byte array which is returned
                        var handler = method.CreateSerializedHandler(obj, serializerLookup, deserializerLookup);
                        return handler;
                    }

                    // Add this handler factory to the list of them, and pair it with this method's corresponding string
                    myHandlerFactoryTuples.Add(Tuple.Create(name, (Func<T, IHandler<byte[], byte[]>>) CreateHandlerFor));
                }

                // Return a new protocol initializer that uses the above list of handler factories
                initializer = new ProtocolInitializer<T>(myHandlerFactoryTuples);
                return true;
            }
            catch
            {
                initializer = null;
                return false;
            }
        }
    }
}

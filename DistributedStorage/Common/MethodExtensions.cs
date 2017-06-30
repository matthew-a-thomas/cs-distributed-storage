namespace DistributedStorage.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Networking.Protocol;

    public static class MethodExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IHandler{TParameter, TResult}"/> which can invoke this <paramref name="method"/> on the given <paramref name="target"/>
        /// by first deserializing parameters from a byte array, and which also serializes any return value and out parameters.
        /// The given <paramref name="serializerLookup"/> and <paramref name="deserializerLookup"/> are used to get serializers and deserializers for the different types relevant to this <paramref name="method"/>
        /// </summary>
        public static IHandler<byte[], byte[]> CreateSerializedHandler(this MethodInfo method, object target, Func<Type, IConverter<object, byte[]>> serializerLookup, Func<Type, IConverter<byte[], object>> deserializerLookup)
        {
            if (!method.DeclaringType.IsInstanceOfType(target))
                throw new Exception($"The given {nameof(target)} cannot have the given {nameof(method)} invoked against it, because their types disagree");

            // Get all the parameters for this method
            var parameters = method.GetParameters();

            // Create a serializer for the return type, if there is one, and for all out parameters
            var hasReturnValue = method.ReturnType != typeof(void);
            var returnTypeSerializer = hasReturnValue ? serializerLookup(method.ReturnType) : null;

            // Create deserializers for the non-out method parameters
            var nonOutParameterDeserializers =
                parameters
                .Select(parameter => parameter.IsOut ? null : parameter.ParameterType)
                .Select(type => type == null ? null : deserializerLookup(type))
                .ToArray();

            // Create serializers for all the out method parameters
            var outParameterSerializers =
                parameters
                .Select(parameter => parameter.IsOut ? parameter.ParameterType : null)
                .Select(type => type == null ? null : serializerLookup(type))
                .ToArray();
            
            // Return the new handler
            return new Handler<byte[], byte[]>(input =>
            {
                // Deserialize the input into a CollectionOfByteArrays for the parameters
                CollectionOfByteArrays serializedParameters;
                using (var stream = new MemoryStream(input))
                {
                    if (!stream.TryRead(out serializedParameters))
                        throw new Exception($"Failed to deserialize a {nameof(CollectionOfByteArrays)} from this input for the parameters");
                    if (serializedParameters.ByteArrays.Count != parameters.Length)
                        throw new Exception("The number of parameters given in the input is not the same as the number of parameters in this method");
                }

                // Deserialize the parameters into a collection of objects
                var deserializedParameters = new object[serializedParameters.ByteArrays.Count];
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    if (parameter.IsOut) // This is an out parameter. Out parameters are populated by invoking the method
                        continue;
                    var serializedParameter = serializedParameters.ByteArrays[i];
                    if (serializedParameter == null && parameter.HasDefaultValue) // This parameter's value wasn't specified, and the parameter has a default value. Use that default value
                        deserializedParameters[i] = parameter.DefaultValue;
                    else if (!nonOutParameterDeserializers[i].TryConvert(serializedParameter, out deserializedParameters[i])) // Otherwise, use the deserializer for this parameter to turn the bytes into something useful
                        throw new Exception($"Failed to deserialize the {i + 1}th parameter");
                }

                // Invoke the method
                var returnValue = method.Invoke(target, deserializedParameters);

                // Set up a return CollectionOfByteArrays for the return parameters, including the return value and out parameters
                var serializedReturn = new List<byte[]>();
                if (hasReturnValue)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (!returnTypeSerializer.TryConvert(returnValue, out var serializedReturnValue))
                        throw new Exception("Failed to serialize the return value of this method");
                    serializedReturn.Add(serializedReturnValue);
                }
                for (var i = 0; i < parameters.Length; ++i)
                {
                    var parameter = parameters[i];
                    if (!parameter.IsOut)
                        continue;
                    var outParameterValue = deserializedParameters[i];
                    if (!outParameterSerializers[i].TryConvert(outParameterValue, out var serializedOutParameterValue))
                        throw new Exception("Failed to serialize an out parameter's value");
                    serializedReturn.Add(serializedOutParameterValue);
                }

                // Serialize and return the return parameters
                using (var stream = new MemoryStream())
                {
                    var collectionOfByteArrays = new CollectionOfByteArrays(serializedReturn);
                    stream.Write(collectionOfByteArrays);
                    var result = stream.ToArray();
                    return result;
                }
            });
        }

        /// <summary>
        /// Returns a strong name for this method
        /// </summary>
        public static string GetStrongName(this MethodInfo method) =>
            $@"{
                method.DeclaringType.AssemblyQualifiedName ?? method.DeclaringType.Name
            }.{
                method.Name
            }<{
                string.Join(",", method.GetGenericArguments().Select(x => x.AssemblyQualifiedName ?? x.Name))
            }>({
                string.Join(
                    ",",
                    method
                    .GetParameters()
                    .Select(x => x.ParameterType.AssemblyQualifiedName ?? x.ParameterType.Name)
                )
            })->{
                method.ReturnType.AssemblyQualifiedName ?? method.ReturnType.Name
            }";
    }
}

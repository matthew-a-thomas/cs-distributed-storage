namespace DistributedStorage.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets an enumerable of all the types that are assignable to <typeparamref name="T"/> in the current assembly
        /// </summary>
        public static IEnumerable<Type> GetTypesAssignableTo<T>() =>
            Assembly
            .GetEntryAssembly()
            .GetTypesAssignableTo<T>();

        /// <summary>
        /// Gets an enumerable of all the types that are assignable to <typeparamref name="T"/> in the given <paramref name="assembly"/>
        /// </summary>
        public static IEnumerable<Type> GetTypesAssignableTo<T>(this Assembly assembly) =>
            assembly
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type));
    }
}

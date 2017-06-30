namespace DistributedStorageTests.Common
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodExtensionsClass
    {
        [TestClass]
        public class GetStrongNameMethod
        {
            [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
            [SuppressMessage("ReSharper", "UnusedTypeParameter")]
            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            private class Example
            {
                public void MethodWithNoParametersAndNoReturnValue() { }

                public void MethodWithParametersAndNoReturnValue(object o) { }

                public object MethodWithNoParametersButWithReturnValue() => new object();

                public void MethodWithTypeParameter<T>() { }

                public void MethodWithOutParameter(out object o) => o = new object();

                public void MethodWithTypedOutParameter<T>(out T value) => value = default(T);
            }

            [TestMethod]
            public void ReturnsDifferentStringsForDifferentKindsOfMethods()
            {
                var strings = new HashSet<string>();
                foreach (var method in typeof(Example).GetMethods())
                {
                    var name = method.GetStrongName();
                    Assert.IsTrue(strings.Add(name), $"This one is a duplicate for {method.Name}: {name}");
                }
            }

            [TestMethod]
            public void WorksForMethodWithNoParametersAndNoReturnValue()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithNoParametersAndNoReturnValue));
                method.GetStrongName();
            }

            [TestMethod]
            public void WorksForMethodWithParametersAndNoReturnValue()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithParametersAndNoReturnValue));
                method.GetStrongName();
            }

            [TestMethod]
            public void WorksForMethodWithNoParametersButWithReturnValue()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithNoParametersButWithReturnValue));
                method.GetStrongName();
            }

            [TestMethod]
            public void WorksForMethodWithTypeParameter()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithTypeParameter));
                method.GetStrongName();
            }

            [TestMethod]
            public void WorksForMethodWithOutParameter()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithOutParameter));
                method.GetStrongName();
            }

            [TestMethod]
            public void WorksForMethodWithTypedOutParameter()
            {
                var method = typeof(Example).GetMethod(nameof(Example.MethodWithTypedOutParameter));
                method.GetStrongName();
            }
        }
    }
}

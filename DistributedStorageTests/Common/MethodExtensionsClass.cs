namespace DistributedStorageTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using DistributedStorage.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodExtensionsClass
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

            public void MethodWithOutParameter(out string o) => o = "";

            public void MethodWithTypedOutParameter<T>(out T value) => value = default(T);

            public int ReadOnlyIntegerProperty => 0;

            public double ReadAndWriteDoubleProperty { get; set; }
        }

        [TestClass]
        public class CreateSerializedHandlerMethod
        {
            [TestMethod]
            public void DoesNotThrowErrorForDifferentMethodTypes()
            {
                var example = new Example();
                foreach (var method in typeof(Example).GetMethods())
                {
                    method.CreateSerializedHandler(example, _ => null, _ => null);
                }
            }

            [TestMethod]
            public void RequestsExpectedSerializersAndDeserializers()
            {
                var requestedSerializers = new HashSet<Type>();
                var requestedDeserializers = new HashSet<Type>();
                var example = new Example();
                foreach (var method in typeof(Example).GetMethods())
                {
                    method.CreateSerializedHandler(
                        example,
                        type =>
                        {
                            requestedSerializers.Add(type);
                            return null;
                        },
                        type =>
                        {
                            requestedDeserializers.Add(type);
                            return null;
                        }
                    );
                }

                foreach (var expectedSerializer in new[]
                {
                    typeof(object),
                    typeof(string),
                    typeof(int),
                    typeof(double)
                })
                    Assert.IsTrue(requestedSerializers.Remove(expectedSerializer), $"The serializer for {expectedSerializer.Name} wasn't requested, but it was supposed to have been");
                Assert.IsTrue(requestedSerializers.Count == 0, $"These serializers were surprisingly requested in addition: {string.Join(", ", requestedSerializers.Select(x => x.Name))}");

                foreach (var expectedDeserializer in new[]
                {
                    typeof(object),
                    typeof(string),
                    typeof(int),
                    typeof(double)
                })
                    Assert.IsTrue(requestedDeserializers.Remove(expectedDeserializer), $"The deserializer for {expectedDeserializer.Name} wasn't requested, but it was supposed to have been");
                Assert.IsTrue(requestedDeserializers.Count == 0, $"These deserializers were surprisingly requested in addition: {string.Join(", ", requestedDeserializers.Select(x => x.Name))}");
            }

            [TestMethod]
            public void ThrowsErrorWhenTargetIsNotCorrectType()
            {
                var testWasSuccessful = false;
                try
                {
                    foreach (var method in typeof(Example).GetMethods())
                    {
                        method.CreateSerializedHandler(this, _ => null, _ => null);
                    }
                }
                catch
                {
                    testWasSuccessful = true;
                }
                Assert.IsTrue(testWasSuccessful);
            }
        }

        [TestClass]
        public class GetStrongNameMethod
        {
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

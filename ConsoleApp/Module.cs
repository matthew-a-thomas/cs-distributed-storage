using Autofac;

namespace ConsoleApp
{
    using System;
    using System.Security.Cryptography;
    using DistributedStorage.Common;
    using DistributedStorage.Encoding;
    using DistributedStorage.Networking;
    using DistributedStorage.Networking.Protocol;
    using DistributedStorage.Networking.Protocol.Methods;
    using DistributedStorage.Networking.Security;
    using DistributedStorage.Networking.Serialization;
    using DistributedStorage.Solving;
    using DistributedStorage.Storage;

    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<Func<HashAlgorithm>>(SHA512.Create).SingleInstance();
            builder.RegisterType<HashVisualizer>().SingleInstance();
            builder.RegisterType<Application>().SingleInstance();

            builder.RegisterType<CryptoRsa>().As<ICryptoRsa>();
            builder.Register(c =>
            {
                var factory = c.Resolve<CryptoSymmetric.Factory>();
                return factory.Create(TimeSpan.FromSeconds(1));
            }).As<ICryptoSymmetric>();
            builder.RegisterType<CryptoSymmetric.Factory>().SingleInstance();
            builder.RegisterType<CryptoEntropy>().As<IEntropy>();
            builder.RegisterType<SecureStreamFactory>().SingleInstance();

            builder.RegisterType<GeneratorFactory>().As<IGeneratorFactory>().SingleInstance();
            builder.RegisterType<ManifestFactory>().As<IManifestFactory>().SingleInstance();
            builder.RegisterType<SolverFactory>().As<ISolverFactory>().SingleInstance();

            builder.RegisterType<StorageFactory>().SingleInstance();
            builder.RegisterType<DatagramProtocol.Factory>().SingleInstance();
            builder.RegisterType<Node.Factory>().SingleInstance();
            builder.RegisterType<Timer>().As<ITimer>().SingleInstance();
            builder.RegisterGeneric(typeof(ProtocolMethodFactory<,>)).SingleInstance();
            builder.RegisterType<NothingSerializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ManifestSerializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ArraySerializer<Manifest>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<IntegerSerializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<SliceSerializer>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ArraySerializer<Slice>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProtocolMethodFactory<Nothing, Manifest[]>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProtocolMethodFactory<Manifest, int>>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ProtocolMethodFactory<Manifest, Slice[]>>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

using Autofac;

namespace ConsoleApp
{
    using System;
    using System.Security.Cryptography;
    using DistributedStorage.Security;

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
            builder.RegisterType<Entropy>().As<IEntropy>();
            builder.RegisterType<SecureStreamFactory>().SingleInstance();
        }
    }
}

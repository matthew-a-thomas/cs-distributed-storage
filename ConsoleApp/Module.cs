using Autofac;

namespace ConsoleApp
{
    using System;
    using System.Security.Cryptography;

    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<Func<HashAlgorithm>>(SHA512.Create).SingleInstance();
            builder.RegisterType<HashVisualizer>().SingleInstance();
            builder.RegisterType<Application>().SingleInstance();
        }
    }
}

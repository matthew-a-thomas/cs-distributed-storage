using Autofac;

namespace DistributedStorage
{
    using System;
    using System.Security.Cryptography;

    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new RandomAdapter(new Random())).As<IRandom>();
            builder.RegisterType<GeneratorFactory>().As<IGeneratorFactory>();
            builder.RegisterType<ManifestFactory>().As<IManifestFactory>();
            builder.RegisterInstance<Func<HashAlgorithm>>(SHA256.Create);
            builder.RegisterType<SolverFactory>().As<ISolverFactory>();
        }
    }
}

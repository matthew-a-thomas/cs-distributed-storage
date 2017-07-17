using Autofac;

namespace Client
{
    internal sealed class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Program>().SingleInstance();
        }
    }
}

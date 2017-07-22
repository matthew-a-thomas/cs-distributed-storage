namespace Client.Modules.RemoteServer
{
    using Autofac;
    using Remote;

    public sealed class RemoteServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RemoteServer.Factory>().SingleInstance();
        }
    }
}

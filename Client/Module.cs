namespace Client
{
    using Autofac;
    using Modules.RemoteServer;
    using Modules.RequestAuthorization;

    internal sealed class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<Modules.Storage>();
            builder.RegisterType<Program>().SingleInstance();
            builder.RegisterModule<RemoteServerModule>();
            builder.RegisterModule<RequestAuthorizationModule>();
        }
    }
}

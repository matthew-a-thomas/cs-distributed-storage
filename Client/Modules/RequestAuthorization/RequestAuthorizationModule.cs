namespace Client.Modules.RequestAuthorization
{
    using Autofac;
    using DistributedStorage.Authorization;
    using Networking.Http;

    public sealed class RequestAuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpRequestMessageAuthorizer>().SingleInstance();
            builder.RegisterType<RequestToAuthorizationTokenFactory>().SingleInstance();
            builder.RegisterType<StringAndAuthorizationTokenAdapter>().SingleInstance();
        }
    }
}

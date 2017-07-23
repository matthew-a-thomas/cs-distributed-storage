namespace Client.Modules
{
    using Autofac;
    using Client.Storage;

    public sealed class Storage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Use on-disk storage of credentials
            builder.RegisterType<UriToCredentialFileBasedContainer>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

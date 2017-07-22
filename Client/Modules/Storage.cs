namespace Client.Modules
{
    using Autofac;
    using DistributedStorage.Authentication;
    using DistributedStorage.Storage.Containers;
    using Remote;

    public sealed class Storage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Use in-memory storage of remote servers
            builder.RegisterType<MemoryAddableContainer<string, IRemoteServer>>().AsImplementedInterfaces().SingleInstance();

            // Use in-memory storage of credentials
            builder.RegisterType<MemoryAddableContainer<string, Credential>>().AsImplementedInterfaces().SingleInstance();
        }
    }
}

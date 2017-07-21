namespace Client.Modules.RemoteServer
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Autofac;
    using DistributedStorage.Storage.Containers;
    using Remote;

    public sealed class Storage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Use in-memory storage of remote servers
            builder.Register(c =>
                {
                    var memory = new ConcurrentDictionary<string, IRemoteServer>();

                    IEnumerable<string> GetKeys() => memory.Keys;

                    bool TryAdd(string key, IRemoteServer value) => memory.TryAdd(key, value);

                    bool TryGet(string key, out IRemoteServer value) => memory.TryGetValue(key, out value);

                    bool TryRemove(string key) => memory.TryRemove(key, out _);

                    var container = new Container<string, IRemoteServer>(new Container<string, IRemoteServer>.Options
                    {
                        GetKeys = GetKeys,
                        TryAdd = TryAdd,
                        TryGet = TryGet,
                        TryRemove = TryRemove
                    });

                    return (IAddableContainer<string, IRemoteServer>)container;
                })
                .SingleInstance();
        }
    }
}

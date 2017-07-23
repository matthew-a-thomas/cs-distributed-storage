namespace Client.Modules
{
    using System.IO;
    using System.Reflection;
    using Autofac;
    using Client.Storage;
    using DistributedStorage.Storage.Containers;
    using DistributedStorage.Storage.FileSystem;
    using Module = Autofac.Module;

    public sealed class Storage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register the "working directory" directory as an IDirectory
            builder.Register(c =>
                {
                    const string workingDirectoryName = "working directory";
                    var assemblyFileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
                    var assemblyFolder = assemblyFileInfo.Directory;
                    var workingDirectory = assemblyFolder.CreateSubdirectory(workingDirectoryName);
                    var adapter = new DirectoryInfoToDirectoryAdapter(workingDirectory);
                    return adapter;
                })
                .AsImplementedInterfaces()
                .SingleInstance();

            // Use on-disk storage of credentials
            builder.Register(c =>
                {
                    const string ownedServerCredentialsDirectoryName = "owned servers";
                    var workingDirectory = c.Resolve<IDirectory>();
                    var ownedServerCredentialsDirectory = workingDirectory.Directories.GetOrCreate(ownedServerCredentialsDirectoryName);
                    var container = new UriToCredentialFileBasedContainer(ownedServerCredentialsDirectory);
                    return container;
                })
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}

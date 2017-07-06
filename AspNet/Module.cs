
namespace AspNet
{
    using Autofac;
    using DistributedStorage.Common;
    using DistributedStorage.Storage;
    using DistributedStorage.Storage.Containers;
    using DistributedStorage.Storage.FileSystem;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using DistributedStorage.Encoding;

    internal class Module : Autofac.Module
    {
        private const string AppDataDirectoryName = "AppData";
        private const string ManifestsDirectoryName = "Manifests";

        protected override void Load(ContainerBuilder builder)
        {
            // Register all Controllers
            foreach (var type in AssemblyExtensions.GetTypesAssignableTo<ControllerBase>())
                builder.RegisterType(type).SingleInstance();

            // Register ManifestsAndSlicesFactoryContainer as the relevant IFactoryContainer
            builder.RegisterType<ManifestsAndSlicesFactoryContainer>().AsImplementedInterfaces().SingleInstance();
            builder.Register(c =>
                {
                    // Register the content root directory as an IDirectory singleton
                    var hostingEnvironment = c.Resolve<IHostingEnvironment>();
                    var contentRoot = new DirectoryInfo(hostingEnvironment.ContentRootPath);
                    var contentRootDirectory = contentRoot.ToDirectory();
                    return contentRootDirectory;
                })
                .SingleInstance();
            builder.Register(c =>
                {
                    // Register the default ManifestsAndSlicesFactoryContainer.Options
                    var contentRootDirectory = c.Resolve<IDirectory>();
                    var appDataDirectory = contentRootDirectory.Directories.GetOrCreate(AppDataDirectoryName);
                    var manifestsDirectory = appDataDirectory.Directories.GetOrCreate(ManifestsDirectoryName);
                    return new ManifestsAndSlicesFactoryContainer.Options(".manifest", ".slice", manifestsDirectory);
                }).SingleInstance();
            builder.Register(c =>
                {
                    // Register the listing of manifests
                    var manifestFactoryContainer = c.Resolve<IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>>>();
                    return manifestFactoryContainer.GetKeys();
                })
                .SingleInstance();
        }
    }
}

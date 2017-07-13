
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
    using Microsoft.AspNetCore.Authorization;
    using Models;
    using Models.Authorization.Handlers;
    using Models.Authorization.Policies;
    using Models.Authorization.Requirements;
    using Models.Manifests;

    internal class Module : Autofac.Module
    {
        private const string
            AppDataDirectoryName = "AppData",
            ManifestExtension = ".manifest",
            ManifestsDirectoryName = "Manifests",
            OwnerFileName = "owner",
            SecretFileName = "secret",
            SliceExtension = ".slice";

        protected override void Load(ContainerBuilder builder)
        {
            // Register all Controllers
            foreach (var type in AssemblyExtensions.GetTypesAssignableTo<ControllerBase>())
                builder.RegisterType(type).SingleInstance();

            // Register ManifestsAndSlicesFactoryContainer as the relevant IFactoryContainer
            builder.RegisterType<ManifestsAndSlicesFactoryContainer>().AsImplementedInterfaces().SingleInstance();
            builder.Register(c =>
                {
                    // Register the AppData directory as an IDirectory singleton
                    var hostingEnvironment = c.Resolve<IHostingEnvironment>();
                    var contentRoot = new DirectoryInfo(hostingEnvironment.ContentRootPath);
                    var contentRootDirectory = contentRoot.ToDirectory();
                    var appDataDirectory = contentRootDirectory.Directories.GetOrCreate(AppDataDirectoryName);
                    return appDataDirectory;
                })
                .SingleInstance();
            builder.Register(c =>
                {
                    // Register the default ManifestsAndSlicesFactoryContainer.Options
                    var appDataDirectory = c.Resolve<IDirectory>();
                    var manifestsDirectory = appDataDirectory.Directories.GetOrCreate(ManifestsDirectoryName);
                    return new ManifestsAndSlicesFactoryContainer.Options(ManifestExtension, SliceExtension, manifestsDirectory);
                })
                .SingleInstance();
            builder.Register(c =>
                {
                    // Register the adapter for IManifestRepository
                    var manifestContainer = c.Resolve<IFactoryContainer<Manifest, IAddableContainer<Hash, Slice>>>();
                    var adapter = new ManifestContainerToManifestRepositoryAdapter(manifestContainer);
                    return (IManifestRepository)adapter;
                })
                .SingleInstance();

            // Authorization handlers
            builder.RegisterType<OwnerOnlyPolicyFactory>().SingleInstance();
            builder.RegisterType<IsOwnerRequirement>().SingleInstance();
            builder.RegisterType<IsOwnerHandler>().As<IAuthorizationHandler>().SingleInstance();

            // Owner repository
            builder.RegisterType<OwnerRepository>().SingleInstance();
            builder.Register(c => new OwnerRepository.Options(OwnerFileName)).SingleInstance();

            // Secret repository
            builder.RegisterType<SecretRepository>().SingleInstance();
            builder.Register(c => new SecretRepository.Options(SecretFileName)).SingleInstance();

            // Token factory
            builder.RegisterType<TokenFactory>().SingleInstance();
        }
    }
}

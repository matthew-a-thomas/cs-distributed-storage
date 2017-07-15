
namespace AspNet
{
    using System;
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
    using Models.Authentication;
    using Models.Authentication.Schemes;
    using Models.Authorization;
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

            // Credential factory
            builder.RegisterType<CredentialFactory>().SingleInstance();

            // String to AuthorizationToken factory
            builder.RegisterType<StringToAuthorizationTokenAdapter>().SingleInstance();

            // Request to AuthorizationToken factory
            builder.RegisterType<RequestToAuthorizationTokenFactory>().SingleInstance();

            // Identity claim factory
            builder.RegisterType<IdentityClaimFactory>().SingleInstance();

            // Authentication scheme
            builder.Register(c =>
                {
                    var secretRepository = c.Resolve<SecretRepository>();
                    var stringToAuthorizationTokenFactory = c.Resolve<StringToAuthorizationTokenAdapter>();
                    var requestToAuthorizationTokenFactory = c.Resolve<RequestToAuthorizationTokenFactory>();
                    var replayAttentionSpan = TimeSpan.FromMinutes(15);

                    var scheme = new MattHttpRequestAuthenticationScheme(
                        secretRepository,
                        stringToAuthorizationTokenFactory,
                        requestToAuthorizationTokenFactory,
                        replayAttentionSpan
                        );

                    return (IHttpRequestAuthenticationScheme) scheme;
                })
                .SingleInstance();

            // Credential to ClaimsIdentity adapter
            builder.RegisterType<CredentialToClaimsIdentityAdapter>().SingleInstance();
        }
    }
}

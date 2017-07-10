namespace AspNet
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using Models.Authorization.Policies;

    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Verifies that certain required types can be resolved from Autofac
        /// </summary>
        private static void AssertSanityChecks(ILifetimeScope container)
        {
            // Verify that all controllers can be created from Autofac
            using (var scope = container.BeginLifetimeScope())
            {
                var unregisteredTypes =
                    DistributedStorage.Common.AssemblyExtensions.GetTypesAssignableTo<ControllerBase>()
                    .Select(type =>
                    {
                        try
                        {
                            if (!scope.IsRegistered(type))
                                return new { Error = new Exception("Cannot resolve type"), Type = type };
                            scope.Resolve(type);
                            return null;
                        }
                        catch (Exception e)
                        {
                            return new { Error = e, Type = type };
                        }
                    })
                    .Where(x => !ReferenceEquals(x, null))
                    .ToList();

                if (unregisteredTypes.Any())
                    throw new Exception(
                        string.Join(
                            Environment.NewLine + Environment.NewLine,
                            new[] { "These controllers cannot be created:" }
                                .Concat(unregisteredTypes.Select(x =>
                                {
                                    var error = x.Error;
                                    while (!ReferenceEquals(error.InnerException, null))
                                        error = error.InnerException;
                                    return x.Type.FullName + " - " + error.Message;
                                }))
                        )
                    );
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ReSharper disable once UnusedMember.Global
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            
            // Add authorization policies
            var ownerOnlyPolicy = new OwnerOnlyPolicyFactory().CreatePolicy();
            services.AddAuthorization(options => options.AddPolicy(OwnerOnlyPolicyFactory.PolicyName, ownerOnlyPolicy));
            
            // Add Autofac
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_hostingEnvironment).SingleInstance(); // Register the hosting environment
            builder.RegisterModule<Module>();
            builder.Populate(services);
            var container = builder.Build();

            // Make sure that all controllers can be instantiated
            AssertSanityChecks(container);

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedParameter.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseMvc();
        }
    }
}

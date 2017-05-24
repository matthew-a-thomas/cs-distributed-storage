namespace ConsoleApp
{
    using Autofac;

    // ReSharper disable once UnusedMember.Global
    internal class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            // Register
            var builder = new ContainerBuilder();
            builder.RegisterModule<Module>();
            using (var container = builder.Build())
            {
                // Resolve
                var app = container.Resolve<Application>();
                app.Run();
            } // Release
            "Press any key to exit . . .".Wait();
        }
    }
}
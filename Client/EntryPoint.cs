namespace Client
{
    using Autofac;

    // ReSharper disable once UnusedMember.Global
    internal partial class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<Module>();
            using (var container = builder.Build())
            {
                var program = container.Resolve<Program>();
                program.Run();
            }
        }
    }
}
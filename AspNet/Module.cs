using Autofac;

namespace AspNet
{
    using DistributedStorage.Common;
    using Microsoft.AspNetCore.Mvc;

    internal class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            foreach (var type in AssemblyExtensions.GetTypesAssignableTo<ControllerBase>())
                builder.RegisterType(type).SingleInstance();
        }
    }
}

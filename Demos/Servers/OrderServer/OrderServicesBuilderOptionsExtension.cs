using System;
using Xpress.Core;

namespace OrderServer
{
    /// <summary>
    /// Order server extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class OrderServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a order server module
        /// </summary>
        public static ServicesBuilderOptions AddOrderServer(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions,typeof(OrderServicesBuilderOptionsExtension).Assembly);

            return servicesBuilderOptions;
        }
    }
}

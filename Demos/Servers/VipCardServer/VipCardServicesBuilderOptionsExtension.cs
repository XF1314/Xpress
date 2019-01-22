using System;
using Xpress.Core;

namespace VipCardServer
{
    /// <summary>
    /// vip card extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class VipCardServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a vip card module
        /// </summary>
        public static ServicesBuilderOptions AddVipCardServer(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions, typeof(VipCardServicesBuilderOptionsExtension).Assembly);
            return servicesBuilderOptions;
        }
    }
}

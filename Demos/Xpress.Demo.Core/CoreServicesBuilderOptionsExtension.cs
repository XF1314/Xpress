using System;
using Xpress.Core;

namespace Xpress.Demo.Core
{
    /// <summary>
    /// Demo core extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class CoreServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a core demo module
        /// </summary>
        public static ServicesBuilderOptions AddDemoCore(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions, typeof(CoreServicesBuilderOptionsExtension).Assembly);
            return servicesBuilderOptions;
        }
    }
}

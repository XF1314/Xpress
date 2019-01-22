using System;
using Xpress.Core;

namespace Xpress.Demo.Application
{
    /// <summary>
    /// Demo application module extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class ApplicationServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a application demo module
        /// </summary>
        public static ServicesBuilderOptions AddDemoApplication(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions, typeof(ApplicationServicesBuilderOptionsExtension).Assembly);
            return servicesBuilderOptions;
        }
    }
}

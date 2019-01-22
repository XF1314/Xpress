using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core;

namespace Xpress.AspNetCore
{
    /// <summary>
    ///AspNetCore extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class AspNetCoreServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a aspNetCore module
        /// </summary>
        public static ServicesBuilderOptions AddAspNetCore(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions, typeof(AspNetCoreServicesBuilderOptionsExtension).Assembly);
            return servicesBuilderOptions;
        }
    }
}

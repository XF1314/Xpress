using System;
using Xpress.Core;

namespace TicketServer
{
    /// <summary>
    /// ticket server extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class TicketServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Add a ticket server module
        /// </summary>
        public static ServicesBuilderOptions AddTicketServer(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister.RegisterAssemblyByBasicInterface(servicesBuilderOptions, typeof(TicketServicesBuilderOptionsExtension).Assembly);
            return servicesBuilderOptions;
        }
    }
}

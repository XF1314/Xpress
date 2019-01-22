using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core;
using Xpress.Core.DependencyInjection;

namespace Xpress.CastleWindsor
{
    /// <summary>
    /// Castle Windsor specific extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class WindsorServicesBuilderExtension
    {
        /// <summary>
        /// Use Castle Windsor as an injection container
        /// </summary>
        public static ServicesBuilderOptions UseWindsor(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister = new WindsorIocRegister();
            servicesBuilderOptions.IocRegister.Register<IIocResolver, WindsorIocResolver>(DependencyLifeStyle.Transient);
            return servicesBuilderOptions;
        }
    }
}

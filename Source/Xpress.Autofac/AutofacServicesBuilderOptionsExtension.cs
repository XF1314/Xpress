using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core;
using Xpress.Core.DependencyInjection;

namespace Xpress.Autofac
{
    /// <summary>
    /// Autofac specific extension methods for <see cref="ServicesBuilderOptions" />.
    /// </summary>
    public static class AutofacServicesBuilderOptionsExtension
    {
        /// <summary>
        /// Use Autofac as an injection container
        /// </summary>
        public static ServicesBuilderOptions UseAutofac(this ServicesBuilderOptions servicesBuilderOptions)
        {
            servicesBuilderOptions.IocRegister = new AutofacIocRegister();
            servicesBuilderOptions.IocRegister.Register<IIocResolver, AutofacIocResolver>(DependencyLifeStyle.Transient);
            return servicesBuilderOptions;
        }
    }
}

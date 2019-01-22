using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core
{
    /// <summary>
    /// Application framework usage
    /// </summary>
    public class AppBuilderOptions
    {
        /// <summary>
        /// Ioc resolver
        /// </summary>
        public readonly IIocResolver IocResolver;

        /// <inheritdoc />
        public AppBuilderOptions(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }
    }
}

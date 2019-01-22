using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xpress.Core;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EventBus.Local;

namespace Xpress.Core
{
    /// <summary>
    /// Xpress application service usage
    /// </summary>
    public static class XpressAppBuilderExtensions
    {
        /// <summary>
        /// Use application framework service
        /// </summary>
        public static void UseXpress(this IApplicationBuilder applicationBuilder, Action<AppBuilderOptions> options = null)
        {
            if (applicationBuilder == null)
            {
                throw new ArgumentNullException(nameof(applicationBuilder));
            }

            var iocResolver = applicationBuilder.ApplicationServices.GetService<IIocResolver>();
            //var localEventBus = iocResolver.Resolve<ILocalEventBus>();
            var builder = new AppBuilderOptions(iocResolver);
            options?.Invoke(builder);
        }
    }
}

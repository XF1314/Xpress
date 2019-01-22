using Microsoft.Extensions.DependencyInjection;
using System;
using Xpress.Core;

namespace Xpress.Core
{
    /// <summary>
    /// Xpress application service injection
    /// </summary>
    public static class XpressServiceCollectionExtensions
    {
        /// <summary>
        /// Add application framework service
        /// </summary>
        public static IServiceProvider AddXpress(this IServiceCollection serviceCollection, Action<ServicesBuilderOptions> options = null)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            var servicesBuilderOptions = new ServicesBuilderOptions();
            options?.Invoke(servicesBuilderOptions);
            return servicesBuilderOptions.Build(serviceCollection);
        }
    }
}


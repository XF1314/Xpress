using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xpress.Core.BackgroundJobs;
using Xpress.Core.DependencyInjection;
using Xpress.Core.EntityFramework;
using Xpress.Core.EventBus.Local;
using Xpress.Core.Repositories;
using Xpress.Core.Uow;

namespace Xpress.Core
{
    /// <summary>
    /// Application framework initialization
    /// </summary>
    public class ServicesBuilderOptions
    {
        /// <summary>
        /// Ioc register
        /// </summary>
        public IocRegisterBase IocRegister { get; set; }

        /// <summary>
        /// Uow options
        /// </summary>
        public DefaultUnitOfWorkOptions UowOptions { get; set; }

        /// <summary>
        /// Local event bus options
        /// </summary>
        public LocalEventBusOptions LocalEventBusOptions { get; set; }

        /// <summary>
        /// Background job options
        /// </summary>
        public BackgroundJobOptions BackgroundJobOptions { get; set; }

        /// <inheritdoc />
        public ServicesBuilderOptions()
        {
            UowOptions = new DefaultUnitOfWorkOptions();
            LocalEventBusOptions = new LocalEventBusOptions();
            BackgroundJobOptions = new BackgroundJobOptions();
        }

        /// <summary>
        /// Build and initialize
        /// </summary>
        public IServiceProvider Build(IServiceCollection services)
        {
            IocRegister.Register(this);
            IocRegister.Register(UowOptions);
            IocRegister.RegisterAssemblyByBasicInterface(this, GetType().Assembly);
            IocRegister.Register(typeof(IRepository<,>), typeof(EFRepositoryBase<,>), DependencyLifeStyle.Transient);
            IocRegister.RegisterInterceptor<UnitOfWorkInterceptor>(implementationType =>
            {
                var isUowNeeded = false;
                if (implementationType.IsDefined(typeof(UnitOfWorkAttribute), true))
                    isUowNeeded = true;
                else
                {
                    var methodInfos = implementationType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    isUowNeeded = methodInfos.Any(m => m.IsDefined(typeof(UnitOfWorkAttribute), true))
                        || UowOptions.ConventionalUowSelectors.Any(selector => selector(implementationType));
                }
                return isUowNeeded;
            });

            services.Configure<LocalEventBusOptions>(x =>
            {
                LocalEventBusOptions.Handlers.ToList().ForEach(y =>
                {
                    if (!x.Handlers.Contains(y))
                    {
                        x.Handlers.Add(y);
                    }
                });
            });

            services.Configure<BackgroundJobOptions>(x =>
            {
                BackgroundJobOptions.GetJobs().ToList()
                .ForEach(y => x.TryAddJob(y));
            });

            return IocRegister.GetServiceProvider(services);
        }
    }
}

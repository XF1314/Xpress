using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.EventBus.Local
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory"/> implementation is used to get/release
    /// handlers using Ioc.
    /// </summary>
    public class IocEventHandlerFactory : IEventHandlerFactory, IDisposable
    {
        public Type HandlerType { get; }

        protected IServiceScope ServiceScope { get; }

        public IocEventHandlerFactory(IServiceScopeFactory scopeFactory, Type handlerType)
        {
            HandlerType = handlerType;
            ServiceScope = scopeFactory.CreateScope();
        }

        /// <summary>
        /// Resolves handler object from Ioc container.
        /// </summary>
        /// <returns>Resolved handler object</returns>
        public IEventHandlerDisposeWrapper GetHandler()
        {
            var scope = ServiceScope.ServiceProvider.CreateScope();
            return new EventHandlerDisposeWrapper(
                (ILocalEventHandler)scope.ServiceProvider.GetRequiredService(HandlerType),
                () => scope.Dispose()
            );
        }

        public void Dispose()
        {
            ServiceScope.Dispose();
        }
    }
}

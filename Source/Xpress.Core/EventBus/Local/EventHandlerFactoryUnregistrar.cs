using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Local
{
    /// <summary>
    /// Used to unregister a <see cref="IEventHandlerFactory"/> on <see cref="Dispose"/> method.
    /// </summary>
    public class EventHandlerFactoryUnregistrar : IDisposable
    {
        private readonly ILocalEventBus _eventBus;
        private readonly Type _eventType;
        private readonly IEventHandlerFactory _factory;

        public EventHandlerFactoryUnregistrar(ILocalEventBus eventBus, Type eventType, IEventHandlerFactory factory)
        {
            _eventBus = eventBus;
            _eventType = eventType;
            _factory = factory;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(_eventType, _factory);
        }
    }
}

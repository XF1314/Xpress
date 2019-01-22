using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Local
{
    public class EventHandlerDisposeWrapper : IEventHandlerDisposeWrapper
    {
        public ILocalEventHandler EventHandler { get; }

        private readonly Action _disposeAction;

        public EventHandlerDisposeWrapper(ILocalEventHandler eventHandler, Action disposeAction = null)
        {
            _disposeAction = disposeAction;
            EventHandler = eventHandler;
        }

        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}

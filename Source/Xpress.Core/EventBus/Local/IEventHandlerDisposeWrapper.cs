using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Local
{
    public interface IEventHandlerDisposeWrapper : IDisposable
    {
        ILocalEventHandler EventHandler { get; }
    }
}

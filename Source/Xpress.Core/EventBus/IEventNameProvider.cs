using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus
{
    public interface IEventNameProvider
    {
        string GetName(Type eventType);
    }
}

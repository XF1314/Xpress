using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Local
{
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// Gets an event handler.
        /// </summary>
        /// <returns>The event handler</returns>
        IEventHandlerDisposeWrapper GetHandler();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus.Local
{
    /// <summary>
    /// This <see cref="IEventHandlerFactory"/> implementation is used to handle events
    /// by a single instance object. 
    /// </summary>
    /// <remarks>
    /// This class always gets the same single instance of handler.
    /// </remarks>
    public class SingleInstanceHandlerFactory : IEventHandlerFactory
    {
        /// <summary>
        /// The event handler instance.
        /// </summary>
        public ILocalEventHandler HandlerInstance { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public SingleInstanceHandlerFactory(ILocalEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandlerDisposeWrapper GetHandler()
        {
            return new EventHandlerDisposeWrapper(HandlerInstance);
        }
    }
}

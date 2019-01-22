using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xpress.Core.EventBus.Cap
{
    public interface ICapEventPublisher 
    {
        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="event">Related data for the event</param>
        /// <returns>The task to handle async operation</returns>
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;

        /// <summary>
        /// Triggers an event.
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="event">Related data for the event</param>
        /// <returns>The task to handle async operation</returns>
        Task PublishAsync(Type eventType, IEvent @event);
    }
}

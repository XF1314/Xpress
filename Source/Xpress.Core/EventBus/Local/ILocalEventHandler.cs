using System.Threading.Tasks;

namespace Xpress.Core.EventBus.Local
{
    public interface ILocalEventHandler
    {
    }

    public interface ILocalEventHandler<in TEvent> : ILocalEventHandler
        where TEvent : IEvent
    {
        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="event">Event data</param>
        Task HandleEventAsync(TEvent @event);
    }
}
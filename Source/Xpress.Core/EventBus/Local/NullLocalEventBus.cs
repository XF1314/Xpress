using System;
using System.Threading.Tasks;

namespace Xpress.Core.EventBus.Local
{
    public sealed class NullLocalEventBus : ILocalEventBus
    {
        public static NullLocalEventBus Instance { get; } = new NullLocalEventBus();

        private NullLocalEventBus()
        {
            
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : IEvent
        {
            return NullDisposable.Instance;
        }

        public IDisposable Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : IEvent
        {
            return NullDisposable.Instance;
        }

        public IDisposable Subscribe<TEvent, THandler>() where TEvent : IEvent where THandler : ILocalEventHandler, new()
        {
            return NullDisposable.Instance;
        }

        public IDisposable Subscribe(Type eventType, ILocalEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : IEvent
        {
            return NullDisposable.Instance;
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            return NullDisposable.Instance;
        }

        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : IEvent
        {
            
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : IEvent
        {
            
        }

        public void Unsubscribe(Type eventType, ILocalEventHandler handler)
        {
            
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : IEvent
        {
            
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            
        }

        public void UnsubscribeAll<TEvent>() where TEvent : IEvent
        {
            
        }

        public void UnsubscribeAll(Type eventType)
        {
            
        }

        public Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            return Task.CompletedTask;
        }

        public Task PublishAsync(Type eventType, IEvent eventData)
        {
            return Task.CompletedTask;
        }
    }
}

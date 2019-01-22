using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xpress.Core.EventBus.Cap
{
    public class NullCapPublisher : ICapEventPublisher
    {
        public static NullCapPublisher Instance { get; } = new NullCapPublisher();

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            await Task.CompletedTask;
        }

        public async Task PublishAsync(Type eventType, IEvent @event)
        {
            await Task.CompletedTask;
        }
    }
}

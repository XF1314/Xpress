using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.EventBus.Cap
{
    public class CapEventPublisher : ICapEventPublisher, ITransientDependency
    {
        private readonly ICapPublisher _capPublisher;

        public CapEventPublisher(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var eventName = eventData.GetEventName();
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException($"{typeof(TEvent).FullName}.{nameof(IEvent.GetEventName)}() is null or empty");
            }

            await _capPublisher.PublishAsync(eventName, eventData);
        }

        public async Task PublishAsync(Type eventType, IEvent eventData)
        {
            var eventName = eventData.GetEventName();
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException($"{eventType.FullName}.{nameof(IEvent.GetEventName)}() is null or empty");
            }

            await _capPublisher.PublishAsync(eventName, eventData);
        }
    }
}

using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Identity;

namespace Xpress.Core.EventBus
{
    public class EventBase : IEvent
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        private readonly string _eventName;

        /// <summary>
        /// 事件Id
        /// </summary>
        public string EventId { get; set; } = GuidProvider.Comb.Create().ToString("N");

        /// <summary>
        /// 事件发布时间
        /// </summary>
        public DateTime EventTime { get; set; } = DateTime.Now;

        public EventBase(string eventName= default(string))
        {
            _eventName = eventName;
        }

        public virtual string GetEventName()
        {
            var eventName = _eventName;
            if (string.IsNullOrWhiteSpace(eventName))
            {
                var eventType = base.GetType();
                if (!eventType.IsGenericType)
                    eventName = EventNameAttribute.GetNameOrDefault(eventType);
                else
                {
                    var eventNameAttribute = GetType().GetAttribute<GenericEventNameAttribute>();
                    eventName = eventNameAttribute.GetName(eventType);
                }
            }

            return eventName;
        }
    }
}

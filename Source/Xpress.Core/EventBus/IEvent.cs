using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.EventBus
{
    public interface IEvent
    {
        /// <summary>
        /// 事件Id
        /// </summary>
        string EventId { get; set; }

        /// <summary>
        /// 事件发布时间
        /// </summary>
        DateTime EventTime { get; set; }


        /// <summary>
        /// 获取事件名称
        /// </summary>
        string GetEventName();
    }
}

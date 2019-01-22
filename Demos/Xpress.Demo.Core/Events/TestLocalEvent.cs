using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.EventBus;

namespace Xpress.Demo.Core.Events
{
    public class TestLocalEvent : EventBase
    {
        public string Message { get; set; }

        public TestLocalEvent(string eventName = default(string)) : base(eventName)
        {

        }
    }
}

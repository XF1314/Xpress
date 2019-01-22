using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.EventBus;

namespace Xpress.Demo.Core.Events
{
    public class TestCapEvent : EventBase
    {
        public string Message { get; set; }

        public TestCapEvent(string eventName= default(string)) : base(eventName)
        {

        }

        public override string GetEventName()
        {
            return "TestCap";
        }
    }
}

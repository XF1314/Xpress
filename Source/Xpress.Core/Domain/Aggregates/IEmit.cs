using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Domain.Aggregates
{
    public interface IEmit<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Apply(TAggregateEvent aggregateEvent);
    }
}

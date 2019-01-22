using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Domain.Aggregates
{
    public interface IEventApplier<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        bool Apply(TAggregate aggregate, IAggregateEvent<TAggregate, TIdentity> aggregateEvent);
    }
}

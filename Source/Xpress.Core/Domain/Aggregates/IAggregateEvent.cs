using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Domain.Aggregates
{
    public interface IAggregateEvent
    {
    }

    public interface IAggregateEvent<TAggregate, TIdentity> : IAggregateEvent
    where TAggregate : IAggregateRoot<TIdentity>
    where TIdentity : IIdentity
    {
    }
}

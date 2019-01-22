using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Extensions;

namespace Xpress.Core.Domain.Aggregates
{
    public abstract class AggregateEvent<TAggregate, TIdentity> : IAggregateEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        public override string ToString()
        {
            return $"{typeof(TAggregate).PrettyPrint()}/{GetType().PrettyPrint()}";
        }
    }
}

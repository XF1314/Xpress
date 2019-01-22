using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Domain.Aggregates
{
    public interface IAggregateRoot
    {

    }

    public interface IAggregateRoot<out TIdentity> : IAggregateRoot
    where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}

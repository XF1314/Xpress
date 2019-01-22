using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.ValueObjects;

namespace Xpress.Core.Domain.Aggregates
{
    public class AggregateName : SingleValueObject<string>, IAggregateName
    {
        public AggregateName(string value) : base(value)
        {
        }
    }
}

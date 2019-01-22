using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Domain.Aggregates
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AggregateNameAttribute : Attribute
    {
        public string Name { get; }

        public AggregateNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
        }
    }
}

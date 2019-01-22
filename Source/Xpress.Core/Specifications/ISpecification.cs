using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);

        IEnumerable<string> WhyIsNotSatisfiedBy(T obj);
    }
}

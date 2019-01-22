using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpress.Core.Specifications
{
    public abstract class Specification<T> : ISpecification<T>
    {
        public bool IsSatisfiedBy(T obj)
        {
            return !IsNotSatisfiedBecause(obj).Any();
        }

        public IEnumerable<string> WhyIsNotSatisfiedBy(T obj)
        {
            return IsNotSatisfiedBecause(obj);
        }

        protected abstract IEnumerable<string> IsNotSatisfiedBecause(T obj);
    }
}

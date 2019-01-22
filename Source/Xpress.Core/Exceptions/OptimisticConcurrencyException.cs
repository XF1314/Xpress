using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Exceptions
{
    public class OptimisticConcurrencyException : Exception
    {
        public OptimisticConcurrencyException(string message)
            : base(message)
        {
        }

        public OptimisticConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

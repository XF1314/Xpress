using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.RetryStrategies
{
    public class OptimisticConcurrencyRetryOption
    {
        public int NumberOfRetriesOnOptimisticConcurrency { get; set; }

        public TimeSpan DelayBeforeRetryOnOptimisticConcurrency { get; set; }
    }
}

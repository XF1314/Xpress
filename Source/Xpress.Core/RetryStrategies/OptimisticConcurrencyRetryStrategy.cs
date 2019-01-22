using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Exceptions;

namespace Xpress.Core.RetryStrategies
{
    public class OptimisticConcurrencyRetryStrategy : IOptimisticConcurrencyRetryStrategy
    {
        private readonly OptimisticConcurrencyRetryOption _optimisticConcurrencyRetryOption;

        public OptimisticConcurrencyRetryStrategy(IOptions<OptimisticConcurrencyRetryOption> optimisticConcurrencyRetryOption)
        {
            _optimisticConcurrencyRetryOption = optimisticConcurrencyRetryOption.Value;
        }

        public Retry ShouldThisBeRetried(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            if (!(exception is OptimisticConcurrencyException))
            {
                return Retry.No;
            }

            return _optimisticConcurrencyRetryOption.NumberOfRetriesOnOptimisticConcurrency >= currentRetryCount
                ? Retry.YesAfter(_optimisticConcurrencyRetryOption.DelayBeforeRetryOnOptimisticConcurrency)
                : Retry.No;
        }
    }
}

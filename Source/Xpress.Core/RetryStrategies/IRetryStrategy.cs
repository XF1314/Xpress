using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.RetryStrategies
{
    public interface IRetryStrategy
    {
        Retry ShouldThisBeRetried(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.ValueObjects;

namespace Xpress.Core.RetryStrategies
{
    public class Retry : ValueObject
    {
        public static Retry Yes { get; } = new Retry(true, TimeSpan.Zero);
        public static Retry YesAfter(TimeSpan retryAfter) => new Retry(true, retryAfter);
        public static Retry No { get; } = new Retry(false, TimeSpan.Zero);

        public bool ShouldBeRetried { get; }
        public TimeSpan RetryAfter { get; }

        private Retry(bool shouldBeRetried, TimeSpan retryAfter)
        {
            if (retryAfter != TimeSpan.Zero && retryAfter != retryAfter.Duration())
                throw new ArgumentOutOfRangeException(nameof(retryAfter));
            if (!shouldBeRetried && retryAfter != TimeSpan.Zero)
                throw new ArgumentException("Invalid combination. Should not be retried and retry after set");

            ShouldBeRetried = shouldBeRetried;
            RetryAfter = retryAfter;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ShouldBeRetried;
            yield return RetryAfter;
        }
    }
}

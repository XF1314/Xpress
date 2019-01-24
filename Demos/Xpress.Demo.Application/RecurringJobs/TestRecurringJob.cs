using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.DependencyInjection;

namespace Xpress.Demo.Application.RecurringJobs
{
    /// <inheritdoc />
    public class TestRecurringJob : IRecurringJob, ITransientDependency
    {
        private readonly ILogger<TestRecurringJob> _logger;

        /// <inheritdoc />
        public TestRecurringJob(ILogger<TestRecurringJob>  logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Job Excution
        /// </summary>
        [RecurringJob("00 02 * * *", "China Standard Time", "default")]
        public void Execute(PerformContext context)
        {
            _logger.LogInformation("RecuringJobTest...");
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.BackgroundJobs;
using Xpress.Core.DependencyInjection;

namespace Xpress.Demo.Application.BackgroundJobs
{
    /// <inheritedoc/>
    public class TestBackGroundJob : IBackgroundJob<TestBackgroundEventArgs>, ITransientDependency
    {
        private readonly ILogger<TestBackGroundJob> _logger;

        /// <inheritedoc/>
        public TestBackGroundJob(ILogger<TestBackGroundJob> logger)
        {
            _logger = logger;
        }

        /// <inheritedoc/>
        public void Execute(TestBackgroundEventArgs args)
        {
            _logger.LogInformation(args.Message);
        }
    }
}

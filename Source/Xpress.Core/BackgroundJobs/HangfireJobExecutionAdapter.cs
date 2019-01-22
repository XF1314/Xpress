using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.BackgroundJobs
{
    public class HangfireJobExecutionAdapter<TArgs>
    {
        protected BackgroundJobOptions Options { get; }
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected IBackgroundJobExecuter JobExecuter { get; }

        public HangfireJobExecutionAdapter(
            IOptions<BackgroundJobOptions> options,
            IBackgroundJobExecuter jobExecuter,
            IServiceScopeFactory serviceScopeFactory)
        {
            JobExecuter = jobExecuter;
            ServiceScopeFactory = serviceScopeFactory;
            Options = options.Value;
        }

        public void Execute(TArgs args)
        {
            using (var scope = ServiceScopeFactory.CreateScope())
            {
                var jobType = Options.GetJob(typeof(TArgs)).JobType;
                var context = new JobExecutionContext(scope.ServiceProvider, jobType, args);
                JobExecuter.Execute(context);
            }
        }
    }
}

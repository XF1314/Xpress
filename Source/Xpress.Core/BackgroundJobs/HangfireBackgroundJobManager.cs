using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.BackgroundJobs
{
    public class HangfireBackgroundJobManager : IBackgroundJobManager, ISingletonDependency
    {
        public Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,TimeSpan? delay = null)
        {
            if (!delay.HasValue)
            {
                return Task.FromResult(
                    BackgroundJob.Enqueue<HangfireJobExecutionAdapter<TArgs>>(
                        adapter => adapter.Execute(args)
                    )
                );
            }
            else
            {
                return Task.FromResult(
                    BackgroundJob.Schedule<HangfireJobExecutionAdapter<TArgs>>(
                        adapter => adapter.Execute(args),
                        delay.Value
                    )
                );
            }
        }
    }
}

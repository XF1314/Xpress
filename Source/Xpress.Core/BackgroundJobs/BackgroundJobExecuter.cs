using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;

namespace Xpress.Core.BackgroundJobs
{
    public class BackgroundJobExecuter : IBackgroundJobExecuter, ITransientDependency
    {
        public ILogger<BackgroundJobExecuter> Logger { protected get; set; }

        protected BackgroundJobOptions Options { get; }

        public BackgroundJobExecuter(IOptions<BackgroundJobOptions> options)
        {
            Options = options.Value;
            Logger = NullLogger<BackgroundJobExecuter>.Instance;
        }

        public virtual void Execute(JobExecutionContext context)
        {
            var job = context.ServiceProvider.GetService(context.JobType);
            if (job == null)
                throw new XpressException("The job type is not registered to DI: " + context.JobType);
            else
            {
                var jobExecuteMethod = context.JobType.GetMethod(nameof(IBackgroundJob<IBackgroundEventArgs>.Execute));
                if (jobExecuteMethod == null)
                    throw new XpressException($"Given job type does not implement {typeof(IBackgroundJob<>).Name}. The job type was: " + context.JobType);
                else
                {
                    try
                    {
                        jobExecuteMethod.Invoke(job, new[] { context.JobArgs });
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw new BackgroundJobExecutionException("A background job execution is failed. See inner exception for details.", ex)
                        {
                            JobType = context.JobType.AssemblyQualifiedName,
                            JobArgs = context.JobArgs
                        };
                    }
                }
            }
        }
    }
}

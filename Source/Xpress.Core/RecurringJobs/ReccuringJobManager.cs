using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.RecurringJobExtensions;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.RecurringJobs
{
    public class ReccuringJobManager : IReccuringJobManager, ISingletonDependency
    {
        /// <summary>
        /// Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        public void AddOrUpdate<TJob>() where TJob : IRecurringJob
        {
            CronJob.AddOrUpdate(typeof(TJob));
        }

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        public void AddOrUpdate(params Type[] types)
        {
            CronJob.AddOrUpdate(types);
        }

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        public void AddOrUpdate(Func<IEnumerable<Type>> typesProvider)
        {
            CronJob.AddOrUpdate(typesProvider);
        }

        /// <summary>
        ///  Builds Hangfire.RecurringJob automatically by using a JSON configuration.
        /// </summary>
        public void AddOrUpdate(string[] jsonFiles, bool reloadOnChange = true)
        {
            CronJob.AddOrUpdate(jsonFiles,reloadOnChange);
        }

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically with the array of Hangfire.RecurringJobExtensions.RecurringJobInfo.
        /// </summary>
        public void AddOrUpdate(params RecurringJobInfo[] recurringJobInfos)
        {
            CronJob.AddOrUpdate(recurringJobInfos);
        }
    }
}

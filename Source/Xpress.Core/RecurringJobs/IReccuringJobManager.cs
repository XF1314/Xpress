using Hangfire.RecurringJobExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.RecurringJobs
{
    public interface IReccuringJobManager
    {
        /// <summary>
        ///  Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        void AddOrUpdate<TJob>() where TJob : IRecurringJob;

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        void AddOrUpdate(params Type[] types);

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically within specified interface or class.
        /// </summary>
        void AddOrUpdate(Func<IEnumerable<Type>> typesProvider);

        /// <summary>
        ///  Builds Hangfire.RecurringJob automatically by using a JSON configuration.
        /// </summary>
        void AddOrUpdate(string[] jsonFiles, bool reloadOnChange = true);

        /// <summary>
        /// Builds Hangfire.RecurringJob automatically with the array of Hangfire.RecurringJobExtensions.RecurringJobInfo.
        /// </summary>
        void AddOrUpdate(params RecurringJobInfo[] recurringJobInfos);
    }
}

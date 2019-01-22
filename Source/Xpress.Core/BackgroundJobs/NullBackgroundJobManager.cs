using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading.Tasks;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.BackgroundJobs
{
    public class NullBackgroundJobManager : IBackgroundJobManager
    {
        public ILogger<NullBackgroundJobManager> Logger { get; set; }

        public static NullBackgroundJobManager Instance => new NullBackgroundJobManager();

        public NullBackgroundJobManager()
        {
            Logger = NullLogger<NullBackgroundJobManager>.Instance;
        }

        public virtual async Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
        {
            Logger.LogInformation("Background job system has not a real implementation. If it's mandatory, use an implementation (either the default provider or a 3rd party implementation). If it's optional, check IBackgroundJobManager.IsAvailable() extension method and act based on it.");

            return await Task.FromResult(string.Empty);
        }
    }
}

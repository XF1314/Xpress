using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.BackgroundJobs
{
    public interface IBackgroundJob
    { }

    /// <summary>
    /// Defines interface of a background job.
    /// </summary>
    public interface IBackgroundJob<in TArgs>: IBackgroundJob where TArgs : IBackgroundEventArgs
    {
        /// <summary>
        /// Executes the job with the <see cref="args"/>.
        /// </summary>
        /// <param name="args">Job arguments.</param>
        void Execute(TArgs args);
    }
}

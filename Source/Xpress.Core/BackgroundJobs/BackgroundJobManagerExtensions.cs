﻿using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Threading;
using Xpress.Core.Utils;

namespace Xpress.Core.BackgroundJobs
{
    /// <summary>
    /// Some extension methods for <see cref="IBackgroundJobManager"/>.
    /// </summary>
    public static class BackgroundJobManagerExtensions
    {
        /// <summary>
        /// Enqueues a job to be executed.
        /// </summary>
        /// <typeparam name="TArgs">Type of the arguments of job.</typeparam>
        /// <param name="backgroundJobManager">Background job manager reference</param>
        /// <param name="args">Job arguments.</param>
        /// <param name="priority">Job priority.</param>
        /// <param name="delay">Job delay (wait duration before first try).</param>
        public static string Enqueue<TArgs>(this IBackgroundJobManager backgroundJobManager, TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TArgs : IBackgroundEventArgs
        {
            return AsyncHelper.RunSync(() => backgroundJobManager.EnqueueAsync<TArgs>(args, priority, delay));
        }

        /// <summary>
        /// Checks if background job system has a real implementation.
        /// It returns false if the current implementation is <see cref="NullBackgroundJobManager"/>.
        /// </summary>
        /// <param name="backgroundJobManager"></param>
        /// <returns></returns>
        public static bool IsAvailable(this IBackgroundJobManager backgroundJobManager)
        {
            return !(CastleProxyHelper.UnProxy(backgroundJobManager) is NullBackgroundJobManager);
        }
    }
}

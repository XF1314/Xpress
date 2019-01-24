using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Identity;

namespace Xpress.Core.BackgroundJobs
{
    public class BackgroundEventArgsBase : IBackgroundEventArgs
    {
        /// <summary>
        /// 事件Id
        /// </summary>
        public string EventId { get; set; } = GuidProvider.Comb.Create().ToString("N");

        /// <summary>
        /// 事件发布时间
        /// </summary>
        public DateTime EventTime { get; set; } = DateTime.Now;
    }
}

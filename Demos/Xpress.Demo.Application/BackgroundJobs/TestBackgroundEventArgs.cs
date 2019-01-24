using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.BackgroundJobs;

namespace Xpress.Demo.Application.BackgroundJobs
{
    public class TestBackgroundEventArgs : BackgroundEventArgsBase
    {
        public string Message { get; set; }
    }
}

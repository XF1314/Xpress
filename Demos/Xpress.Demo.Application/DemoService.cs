using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Application;

namespace Xpress.Demo.Application
{
    public class DemoService : ApplicationService, IDemoService
    {
        public ILogger<DemoService> Logger { get; set; }

        public DemoService(ILogger<DemoService> logger)
        {
            Logger = NullLogger<DemoService>.Instance;
        }

        public void WriteMessage(string message)
        {
            Logger.LogCritical(message);
        }
    }
}

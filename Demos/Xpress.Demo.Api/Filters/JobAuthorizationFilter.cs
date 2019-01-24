using Exceptionless;
using Exceptionless.Logging;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xpress.Demo.Api.Filters
{
    /// <inheritdoc />
    public class JobAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <inheritdoc />
        public JobAuthorizationFilter(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <inheritdoc />
        public bool Authorize(DashboardContext context)
        {
            if (_hostingEnvironment.IsProduction()) //非测试环境
                return false;
            else
            {
                ExceptionlessClient.Default
                    .CreateLog(nameof(JobAuthorizationFilter), $"job请求访问地址：{context.Request.RemoteIpAddress}", LogLevel.Fatal).Submit();
                return true;
            }
        }
    }
}

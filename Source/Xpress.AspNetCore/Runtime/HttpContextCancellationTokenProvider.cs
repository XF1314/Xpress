using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Threading;

namespace Xpress.AspNetCore.Runtime
{
    public class HttpContextCancellationTokenProvider : ICancellationTokenProvider, ITransientDependency
    {
        public CancellationToken Token => _httpContextAccessor.HttpContext?.RequestAborted ?? default(CancellationToken);

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCancellationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}

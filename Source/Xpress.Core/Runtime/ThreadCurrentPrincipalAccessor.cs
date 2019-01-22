using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.Runtime
{
    public class ThreadCurrentPrincipalAccessor : ICurrentPrincipalAccessor, ISingletonDependency
    {
        public virtual ClaimsPrincipal Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}

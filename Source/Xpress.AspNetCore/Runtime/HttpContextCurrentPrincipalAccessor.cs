using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xpress.Core.Runtime;

namespace Xpress.AspNetCore.Runtime
{
    public class HttpContextCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
    {
        public override ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? base.Principal;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}

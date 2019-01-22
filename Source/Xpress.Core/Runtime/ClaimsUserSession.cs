using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xpress.Core.DependencyInjection;

namespace Xpress.Core.Runtime
{
    /// <summary>
    /// 使用 <see cref="ClaimsPrincipal"/> 获取当前用户
    /// </summary>
    public class ClaimsUserSession : IUserSession, ITransientDependency
    {
        /// <inheritdoc />
        public string UserId => _principalAccessor.Principal?.Claims.GetValue(XpressClaimTypes.UserIdentity);

        /// <inheritdoc />
        public string UserName => _principalAccessor.Principal?.Claims.GetValue(ClaimTypes.GivenName);

        /// <inheritdoc />
        public string UserType => _principalAccessor.Principal?.Claims.GetValue(XpressClaimTypes.UserType);


        private readonly ICurrentPrincipalAccessor _principalAccessor;

        /// <summary>
        /// 创建 <see cref="ClaimsUserSession"/>
        /// </summary>
        public ClaimsUserSession(ICurrentPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }
    }
}

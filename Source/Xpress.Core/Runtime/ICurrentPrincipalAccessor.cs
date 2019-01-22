using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Xpress.Core.Runtime
{
    /// <summary>
    /// 获取当前身份凭证 <see cref="ClaimsPrincipal"/> 接口
    /// </summary>
    public interface ICurrentPrincipalAccessor
    {
        /// <summary>
        /// 获取当前身份凭证
        /// </summary>
        ClaimsPrincipal Principal { get; }
    }
}

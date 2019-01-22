using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Xpress.Core.Runtime
{
    /// <summary>
    /// 扩展<see cref="Claim"/>方法
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        /// 获取单个值
        /// </summary>
        public static string GetValue(this IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(p => p.Type == type)?.Value;
        }
    }
}

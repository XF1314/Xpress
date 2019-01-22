using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Runtime
{
    public   interface IUserSession
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// 获取当前用户名称
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 获取当前用户类型
        /// </summary>
        string UserType { get; }
    }
}

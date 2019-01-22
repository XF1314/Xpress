using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Runtime
{
    /// <inheritdoc />
    public class NullUserSession : IUserSession
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        public static NullUserSession Instanse = new NullUserSession();

        /// <inheritdoc />
        public string UserId { get; } = null;

        /// <inheritdoc />
        public string UserName { get; } = null;

        /// <inheritdoc />
        public string UserType { get; } = null;
    }
}

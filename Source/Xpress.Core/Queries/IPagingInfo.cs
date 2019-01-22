using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Queries
{
    public interface IPagingInfo
    {
        /// <summary>
        /// 页号，从 1 开始
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        int PageSize { get; set; }
    }
}

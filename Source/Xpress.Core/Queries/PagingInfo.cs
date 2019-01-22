using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Queries
{
    public class PagingInfo : IPagingInfo
    {
        /// <summary>
        /// 页号，从 1 开始
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 初始化 <see cref="PageInfo" /> 类的新实例。
        /// </summary>
        public PagingInfo(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}

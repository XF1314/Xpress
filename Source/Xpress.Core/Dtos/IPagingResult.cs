using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Queries;

namespace Xpress.Core.Dtos
{
    public interface IPagingResult : IPagingInfo
    {
        /// <summary>
        /// 总数
        /// </summary>
        int TotalCount { get; set; }
    }
}

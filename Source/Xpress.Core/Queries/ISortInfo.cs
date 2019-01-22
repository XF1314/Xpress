using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Queries
{
    /// <summary>
    /// 排序信息
    /// </summary>
    public interface ISortInfo
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        /// <example>['SortNo desc', 'CreateTime desc']</example>
        IList<string> SortFields { get; set; }
    }
}

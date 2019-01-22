using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Queries
{
    /// <summary>
    /// 分页排序信息
    /// </summary>
    public class PagingSortInfo : PagingInfo, IPagingSortInfo
    {
        /// <summary>
        /// 分页排序信息
        /// </summary>
        public PagingSortInfo()
        {
            SortFields = new List<string>();
        }

        /// <summary>
        /// 分页排序信息
        /// </summary>
        public PagingSortInfo(int pageIndex, int pageSize = 10, IEnumerable<string> sort = null)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;

            var sortFields = new List<string>();
            if (sort != null) sortFields.AddRange(sort);

            SortFields = sortFields;
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        /// <example>['SortNo desc', 'CreateTime desc']</example>
        public IList<string> SortFields { get; set; }
    }
}

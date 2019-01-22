using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Queries
{
    /// <summary>
    /// 排序查询
    /// </summary>
    public class SortQuery : Query, ISortInfo
    {
        /// <inheritdoc />
        public IList<string> SortFields { get; set; } = new List<string>();
    }
}

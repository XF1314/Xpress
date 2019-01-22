using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpress.Core.Queries;

namespace Xpress.Core.Dtos
{
    public class PagingResult<TData> : Result<List<TData>>, IPagingResult
    {
        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PagingResult() { }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PagingResult(IList<TData> data, int totalCount, int pageIndex, int pageSize)
            : base(data.ToList())
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public PagingResult(IList<TData> data, int totalCount, IPagingInfo pager)
            : this(data, totalCount, pager?.PageIndex ?? 0, pager?.PageSize ?? 0)
        {
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public PagingResult(ResultCode code, string message = null)
            : base(code, message)
        {
        }

        /// <summary>
        /// 当前页
        /// </summary>
        /// <example>1</example>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        /// <example>30</example>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        /// <example>5</example>
        public int TotalPage => PageSize > 0 ? (int)Math.Ceiling((decimal)TotalCount / PageSize) : 0;

        /// <summary>
        /// 数据总条数
        /// </summary>
        /// <example>250</example>
        public int TotalCount { get; set; }

        public PagingResult<T> FromError<T>(string v)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Dtos
{
    public interface IResult
    {
        /// <summary>
        /// 结果状态码
        /// </summary>
        ResultCode Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <example>操作成功</example>
        string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        bool IsSuccess { get; }
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    public interface IResult<TData> : IResult
    {
        /// <summary>
        /// 结果状态码
        /// </summary>
        TData Data { get; set; }
    }
}

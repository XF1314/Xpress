using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Dtos
{
    /// <summary>
    /// 错误结果
    /// </summary>
    public class ErrorResult : Result
    {
        /// <inheritdoc />
        public ErrorResult() : base(ResultCode.Fail)
        {
            Error = new Dictionary<string, object>();
        }

        /// <inheritdoc />
        public ErrorResult(IDictionary<string, object> error, ResultCode code = ResultCode.Fail, string message = null)
        {
            Code = code;
            Message = message;
            Error = error;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public IDictionary<string, object> Error { get; set; }
    }
}

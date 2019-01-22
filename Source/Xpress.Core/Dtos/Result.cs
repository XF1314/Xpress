using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Utils;

namespace Xpress.Core.Dtos
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        public const string SuccessCode = "Success";


        private string _message;

        /// <summary>
        /// 返回结果
        /// </summary>
        public Result()
        {
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public Result(ResultCode code, string message = null)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 结果状态码
        /// </summary>
        public ResultCode Code { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <example>操作成功</example>
        public string Message
        {
            get { return _message ?? Code.DisplayName(); }
            set { _message = value; }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => Code == ResultCode.Ok;

        #region 静态函数

        /// <summary>
        /// 返回指定 Code
        /// </summary>
        public static Result FromCode(ResultCode code, string message = null)
        {
            return new Result(code, message);
        }

        /// <summary>
        /// 返回指定 Code
        /// </summary>
        public static Result<T> FromCode<T>(ResultCode code, string message = null)
        {
            return new Result<T>(code, message);
        }

        /// <summary>
        /// 返回异常信息
        /// </summary>
        public static Result FromError(string message, ResultCode code = ResultCode.Fail)
        {
            return new Result(code, message);
        }

        /// <summary>
        /// 返回异常信息
        /// </summary>
        public static Result<T> FromError<T>(string message, ResultCode code = ResultCode.Fail)
        {
            return new Result<T>(code, message);
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public static Result FromResult(IResult result)
        {
            return new Result(result.Code, result.Message);
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public static Result<T> FromResult<T>(IResult result)
        {
            return new Result<T>(result.Code, result.Message);
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public static Result<T> FromResult<T>(IResult result, T data)
        {
            return new Result<T>(result.Code, result.Message) { Data = data };
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public static Result<T> FromResult<T>(IResult<T> result)
        {

            return new Result<T>(result.Code, result.Message) { Data = result.Data };
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public static Result<T> FromData<T>(T data)
        {
            return new Result<T>(data);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static Result Ok(string message = null)
        {
            return FromCode(ResultCode.Ok, message);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static Result<T> Ok<T>(T data)
        {
            return FromData(data);
        }

        #endregion
    }

    public class Result<TData> : Result, IResult<TData>
    {
        public Result()
        {
        }

        public Result(TData data)
            : base(ResultCode.Ok)
        {
            Data = data;
        }

        /// <summary>
        /// 结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public Result(ResultCode code, string message = null)
            : base(code, message)
        {

        }

        /// <summary>
        /// 结果数据
        /// </summary>
        public TData Data { get; set; }
    }
}

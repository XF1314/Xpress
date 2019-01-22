using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpress.Core.Dtos
{
    public enum ResultCode
    {        /// <summary>
             /// 操作成功
             ///</summary>
        [Display(Name = "操作成功", GroupName = Result.SuccessCode)]
        Ok = 0,

        /// <summary>
        /// 操作失败
        ///</summary>
        [Display(Name = "操作失败")]
        Fail = 1,

        /// <summary>
        /// 服务数据异常
        ///</summary>
        [Display(Name = "服务数据异常")]
        ServerError = 10,

        /// <summary>
        /// 未登录
        ///</summary>
        [Display(Name = "未登录")]
        Unauthorized = 20,

        /// <summary>
        /// 未授权
        /// </summary>
        [Display(Name = "未授权")]
        Forbidden = 21,

        /// <summary>
        /// Token 失效
        /// </summary>
        [Display(Name = "Token 失效")]
        InvalidToken = 22
    }
}

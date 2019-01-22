using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpress.Core.Extensions
{
    public enum ComparisonOperator
    {
        /// <summary>
        /// 等于
        /// </summary>
        [Display(Name = "等于")]
        Equal,

        /// <summary>
        /// 不等于
        /// </summary>
        [Display(Name = "不等于")]
        NotEqual,

        /// <summary>
        /// 模糊匹配
        /// </summary>
        [Display(Name = "模糊匹配")]
        Like,

        /// <summary>
        /// 不包含模糊匹配
        /// </summary>
        [Display(Name = "不包含模糊匹配")]
        NotLike,

        /// <summary>
        /// 以...开头
        /// </summary>
        [Display(Name = "以...开头")]
        StartWidth,

        /// <summary>
        /// 小于
        /// </summary>
        [Display(Name = "小于")]
        LessThan,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Display(Name = "小于等于")]
        LessThanOrEqual,

        /// <summary>
        /// 大于
        /// </summary>
        [Display(Name = "大于")]
        GreaterThan,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Display(Name = "大于等于")]
        GreaterThanOrEqual,

        /// <summary>
        /// 在...之间，属性必须是一个集合（或逗号分隔的字符串），取第一和最后一个值。
        /// </summary>
        [Display(Name = "在...之间")]
        Between,

        /// <summary>
        /// 大于等于起始，小于结束，属性必须是一个集合（或逗号分隔的字符串），取第一和最后一个值。
        /// </summary>
        [Display(Name = "大于等于起始，小于结束")]
        GreaterEqualAndLess,

        /// <summary>
        /// 包含，属性必须是一个集合（或逗号分隔的字符串）
        /// </summary>
        [Display(Name = "包含")]
        Include,

        /// <summary>
        /// 不包含，属性必须是一个集合（或逗号分隔的字符串）
        /// </summary>
        [Display(Name = "不包含")]
        NotInclude,

        /// <summary>
        /// 为空或不为空，可以为 bool类型，或可空类型。
        /// </summary>
        [Display(Name = "为空或不为空")]
        IsNull,

        /// <summary>
        /// 是否包含指定枚举
        /// </summary>
        [Display(Name = "是否包含指定枚举")]
        HasFlag,
    }
}

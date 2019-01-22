using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Extensions;

namespace Xpress.Core.Queries
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class QueryAttribute : Attribute
    {
        /// <summary>
        /// 比较操作符
        /// </summary>
        public ComparisonOperator ComparisonOperator { get; set; }

        /// <summary>
        /// 对应属性路径
        /// </summary>
        public string[] PropertyPath { get; set; }

        /// <summary>
        /// 或条件组
        /// </summary>
        public string OrGroup { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryAttribute(params string[] propertyPath)
        {
            PropertyPath = propertyPath;
        }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryAttribute(ComparisonOperator comparisonOperator, params string[] propertyPath)
        {
            PropertyPath = propertyPath;
            ComparisonOperator = comparisonOperator;
        }
    }
}

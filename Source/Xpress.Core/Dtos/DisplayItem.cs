using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Dtos
{
    public class DisplayItem
    {
        /// <summary>
        /// Id标识
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// 键值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据类型(string,bool,int,html,decimal)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 显示说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 列的排序权重
        /// </summary>
        public int? Order { get; set; }
    }

    /// <inheritdoc />
    public class DisplayItem<TValue> : DisplayItem
    {
        /// <summary>
        /// 值
        /// </summary>
        public new TValue Id
        {
            get => (TValue)base.Id;
            set => base.Id = value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Xpress.Core.Queries
{
    public interface IQuery
    {
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <typeparam name="TEntity">要查询的实体类型</typeparam>
        Expression<Func<TEntity, bool>> GetFilter<TEntity>() where TEntity : class;

        void And<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }
}

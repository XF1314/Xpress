using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xpress.Core.Dtos;
using Xpress.Core.Queries;

namespace Xpress.Core.Extensions
{
    public static class QueryableExtensions
    {
        public static Task<TDto> FirstOrDefaultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery query)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();

            if (query is ISortInfo sort)
            {
                data = data.OrderBy(sort);
            }

            return data.FirstOrDefaultAsync();
        }

        public static Task<TDto> FirstOrDefaultAsync<TEntity, TDto>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> queryExpression)
            where TEntity : class
        {
            var data = source.Where(queryExpression).ProjectTo<TDto>();
            return data.FirstOrDefaultAsync();
        }

        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery query, ISortInfo sort, string defaultSort = null)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>().OrderBy(sort, defaultSort);
            return data.ToListAsync();
        }

        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> query, ISortInfo sort, string defaultSort = null)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>().OrderBy(sort, defaultSort);
            return data.ToListAsync();
        }

        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, ISortInfo sort, string defaultSort = null)
            where TEntity : class
        {
            var data = source.ProjectTo<TDto>().OrderBy(sort, defaultSort);
            return data.ToListAsync();
        }

        public static Task<List<TDto>> ToListAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery query)
            where TEntity : class
        {
            var data = source.Where(query).ProjectTo<TDto>();
            return data.ToListAsync();
        }

        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="defaultSort">默认排序</param>
        public static Task<PagingResult<TDto>> ToPagingResultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IPagingQuery query, string defaultSort = null)
            where TEntity : class
        {
            return ToPagingResultAsync<TEntity, TDto>(source, query, query, defaultSort);
        }

        /// <summary>
        /// 查询指定条件的数据
        /// </summary>
        /// <typeparam name="TEntity">查询的实体</typeparam>
        /// <typeparam name="TDto">返回的类型</typeparam>
        /// <param name="source"></param>
        /// <param name="query">查询条件</param>
        /// <param name="page">分页信息</param>
        /// <param name="defaultSort">默认排序</param>
        public static async Task<PagingResult<TDto>> ToPagingResultAsync<TEntity, TDto>(this IQueryable<TEntity> source, IQuery query, IPagingSortInfo page, string defaultSort = null)
            where TEntity : class
        {
            page = page ?? new PagingSortInfo();
            var pageIndex = Math.Max(1, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            var result = new PagingResult<TDto>() { PageIndex = pageIndex, PageSize = pageSize };
            var data = source.Where(query);
            result.TotalCount = await data.CountAsync();
            if (result.TotalCount > 0)
            {
                var mapData = data.OrderBy(page, defaultSort).ProjectTo<TDto>();
                result.Data = await mapData.Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToListAsync();
            }

            return result;
        }

        /// <summary>
        /// 返回分页的数据数据
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source"></param>
        /// <param name="page">分页信息</param>
        /// <param name="defaultSort">默认排序</param>
        public static async Task<PagingResult<TEntity>> ToPagingResultAsync<TEntity>(this IQueryable<TEntity> source, IPagingSortInfo page, string defaultSort = null)
            where TEntity : class
        {
            page = page ?? new PagingSortInfo();
            var pageIndex = Math.Max(1, page.PageIndex);
            var pageSize = Math.Max(1, page.PageSize);

            var result = new PagingResult<TEntity>() { PageIndex = pageIndex, PageSize = pageSize };
            result.TotalCount = await source.CountAsync();
            if (result.TotalCount > 0)
            {
                var mapData = source.OrderBy(page, defaultSort);
                result.Data = await mapData.Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize).ToListAsync();
            }

            return result;
        }

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, IQuery query)
            where TEntity : class
        {
            var filter = query?.GetFilter<TEntity>();
            if (filter != null)
                source = source.Where(filter);
            return source;
        }

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, ISortInfo sort, string defaultSort = null)
        {
            var sortFields = sort?.SortFields?.Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.StartsWith("+") ? (p.TrimStart('+') + " ASC") : p.StartsWith("-") ? (p.TrimStart('-') + " DESC") : p).ToArray();
            if (sortFields != null && sortFields.Length > 0)
            {
                source = source.OrderBy(string.Join(" , ", sortFields));
            }
            else if (!string.IsNullOrEmpty(defaultSort))
            {
                source = source.OrderBy(defaultSort);
            }

            return source;
        }

        /// <summary>
        /// 返回分页后的数据
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="pageIndex">第几页，从1开始</param>
        /// <param name="pageSize">每页多少条数据，不能小于1</param>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (pageIndex < 1)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "页不能小于1");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "每页大小不能小于1");

            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 如果条件 <paramref name="condition"/> 为 true， 根据指定的过滤器 <paramref name="predicate"/> 筛选集合
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="condition">是否过滤</param>
        /// <param name="predicate">过滤器</param>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

        /// <summary>
        /// 如果条件 <paramref name="condition"/> 为 true， 根据指定的过滤器 <paramref name="predicate"/> 筛选集合
        /// </summary>
        /// <param name="query">查询的数据集</param>
        /// <param name="condition">是否过滤</param>
        /// <param name="predicate">过滤器</param>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            return condition
                ? query.Where(predicate)
                : query;
        }

    }
}

using Castle.Core.Internal;
using Castle.DynamicProxy.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xpress.Core.Extensions;

namespace Xpress.Core.Queries
{
    public static class QueryExtensions
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, List<ValueTuple<PropertyInfo, QueryAttribute>>>>
            _queryCache = new ConcurrentDictionary<Type, Dictionary<string, List<(PropertyInfo, QueryAttribute)>>>();

        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <typeparam name="TEntity">要查询的实体类型</typeparam>
        public static Expression<Func<TEntity, bool>> GetQueryExpression<TEntity>(this IQuery query)
            where TEntity : class
        {
            if (query == null) return null;

            var queryType = query.GetType();
            var entityParam = Expression.Parameter(typeof(TEntity), "o");

            Expression body = null;

            var groupQuery = GetQueryGroup(queryType);

            foreach (var group in groupQuery.Values)
            {
                Expression sub = null;

                foreach ((var property, var attr) in group)
                {
                    var value = property.GetValue(query);
                    if (value is string str)
                    {
                        str = str.Trim();
                        value = string.IsNullOrEmpty(str) ? null : str;
                    }

                    var experssion = CreateQueryExpression(entityParam, value, attr.PropertyPath, attr.ComparisonOperator);
                    if (experssion != null)
                    {
                        sub = sub == null ? experssion : Expression.OrElse(sub, experssion);
                    }
                }

                if (sub != null)
                {
                    body = body == null ? sub : Expression.AndAlso(body, sub);
                }
            }

            return body != null ? Expression.Lambda<Func<TEntity, bool>>(body, entityParam) : null;
        }

        private static Dictionary<string, List<ValueTuple<PropertyInfo, QueryAttribute>>> GetQueryGroup(Type type)
        {
            return _queryCache.GetOrAdd(type, queryType =>
            {
                var groupIndex = 0;
                var groupQuery = new Dictionary<string, List<ValueTuple<PropertyInfo, QueryAttribute>>>();
                var properties = queryType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic |
                                                         BindingFlags.Public);
                foreach (var property in properties)
                {
                    var queries = property.GetAttributes<QueryAttribute>().ToArray();
                    if (queries.Length == 0) continue;

                    foreach (var attr in queries)
                    {
                        if (attr.OrGroup == null)
                            attr.OrGroup = groupIndex.ToString();
                        if (attr.PropertyPath == null || attr.PropertyPath.Length == 0)
                            attr.PropertyPath = new[] { property.Name };

                        var group = groupQuery.GetOrAdd(attr.OrGroup, p => new List<(PropertyInfo, QueryAttribute)>());
                        group.Add((property, attr));
                    }
                    groupIndex++;
                }
                return groupQuery;
            });
        }

        private static Expression CreateQueryExpression(Expression entityParan, object value, string[] propertyPath, ComparisonOperator comparisonOperator)
        {
            var member = CreatePropertyExpression(entityParan, propertyPath);

            switch (comparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return CreateEqualExpression(member, value);
                case ComparisonOperator.NotEqual:
                    return CreateNotEqualExpression(member, value);
                case ComparisonOperator.Like:
                    return CreateLikeExpression(member, value);
                case ComparisonOperator.NotLike:
                    return CreateNotLikeExpression(member, value);
                case ComparisonOperator.StartWidth:
                    return CreateStartsWithExpression(member, value);
                case ComparisonOperator.LessThan:
                    return CreateLessThanExpression(member, value);
                case ComparisonOperator.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(member, value);
                case ComparisonOperator.GreaterThan:
                    return CreateGreaterThanExpression(member, value);
                case ComparisonOperator.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(member, value);
                case ComparisonOperator.Between:
                    return CreateBetweenExpression(member, value);
                case ComparisonOperator.GreaterEqualAndLess:
                    return CreateGreaterEqualAndLessExpression(member, value);
                case ComparisonOperator.Include:
                    return CreateIncludeExpression(member, value);
                case ComparisonOperator.NotInclude:
                    return CreateNotIncludeExpression(member, value);
                case ComparisonOperator.IsNull:
                    return CreateIsNullExpression(member, value);
                case ComparisonOperator.HasFlag:
                    return CreateHasFlagExpression(member, value);
                default:
                    return null;
            }
        }

        private static MemberExpression CreatePropertyExpression(Expression param, string[] propertyPath)
        {
            var expression = propertyPath.Aggregate(param, Expression.Property) as MemberExpression;
            return expression;
        }


        private static Expression CreateEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CreateQueryParamExpression(ChangeType(value, member.Type), member.Type);

            return Expression.Equal(member, val);
        }


        private static Expression CreateNotEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CreateQueryParamExpression(ChangeType(value, member.Type), member.Type);

            return Expression.NotEqual(member, val);
        }

        private static Expression CreateLikeExpression(MemberExpression member, object value)
        {
            if (value == null) return null;
            if (member.Type != typeof(string))
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");

            var str = value.ToString();
            var val = CreateQueryParamExpression(str, member.Type);

            return Expression.Call(member, nameof(string.Contains), null, val);
        }

        private static Expression CreateNotLikeExpression(MemberExpression member, object value)
        {
            var like = CreateLikeExpression(member, value);
            if (like == null) return null;

            return Expression.Not(like);
        }

        private static Expression CreateStartsWithExpression(MemberExpression member, object value)
        {
            if (value == null) return null;
            if (member.Type != typeof(string))
                throw new ArgumentOutOfRangeException(nameof(member), $"Member '{member}' can not use 'Like' compare");

            var str = value.ToString();
            var val = CreateQueryParamExpression(str, member.Type);

            return Expression.Call(member, nameof(string.StartsWith), null, val);
        }

        private static Expression CreateLessThanExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var right = CreateQueryParamExpression(ChangeType(value, val.Type), val.Type);

            return Expression.LessThan(val, right);
        }

        private static Expression CreateLessThanOrEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var right = CreateQueryParamExpression(ChangeType(value, val.Type), val.Type);

            return Expression.LessThanOrEqual(val, right);
        }

        private static Expression CreateGreaterThanExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var right = CreateQueryParamExpression(ChangeType(value, val.Type), val.Type);

            return Expression.GreaterThan(val, right);
        }

        private static Expression CreateGreaterThanOrEqualExpression(MemberExpression member, object value)
        {
            if (value == null) return null;

            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var right = CreateQueryParamExpression(ChangeType(value, val.Type), val.Type);

            return Expression.GreaterThanOrEqual(val, right);
        }

        private static Expression CreateBetweenExpression(MemberExpression member, object value)
        {
            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var list = GetListValue(val.Type, value);
            if (list == null) return null;
            if (list.Count < 2) return null;

            var left = list[0];
            var right = list[list.Count - 1];
            if (left == null || right == null) return null;

            var leftVal = CreateQueryParamExpression(left, val.Type);
            var rightVal = CreateQueryParamExpression(right, val.Type);

            return Expression.AndAlso(Expression.GreaterThanOrEqual(val, leftVal),
                Expression.LessThanOrEqual(val, rightVal));
        }

        private static Expression CreateGreaterEqualAndLessExpression(MemberExpression member, object value)
        {
            var val = CheckConvertToEnumUnderlyingType(member.Type, member);
            var list = GetListValue(val.Type, value);
            if (list == null) return null;
            if (list.Count < 2) return null;

            var left = list[0];
            var right = list[list.Count - 1];
            if (left == null || right == null) return null;

            var leftVal = CreateQueryParamExpression(left, val.Type);
            var rightVal = CreateQueryParamExpression(right, val.Type);

            return Expression.AndAlso(Expression.GreaterThanOrEqual(val, leftVal),
                Expression.LessThan(val, rightVal));
        }

        private static Expression CheckConvertToEnumUnderlyingType(Type memberType, Expression val)
        {
            var nonNullableType = memberType.GetNonNullableType();
            if (nonNullableType.IsEnum)
            {
                var underlyingType = nonNullableType.GetEnumUnderlyingType();
                if (memberType.IsNullableType())
                {
                    underlyingType = underlyingType.GetTypeOfNullable();
                }

                return Expression.Convert(val, underlyingType);
            }
            return val;
        }

        private static Expression CreateIncludeExpression(MemberExpression member, object value)
        {
            var list = GetListValue(member.Type, value);
            if (list == null || list.Count == 0) return null;
            if (list.Count == 1)
            {
                return CreateEqualExpression(member, list[0]);
            }

            var listType = typeof(IEnumerable<>).MakeGenericType(member.Type);
            var vals = CreateQueryParamExpression(list, listType);

            return Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), new[] { member.Type }, vals, member);
        }

        private static Expression CreateNotIncludeExpression(MemberExpression member, object value)
        {
            var includeExpression = CreateIncludeExpression(member, value);
            if (includeExpression == null) return null;

            return Expression.Not(includeExpression);
        }

        private static Expression CreateIsNullExpression(MemberExpression member, object value)
        {
            if (member.Type.IsValueType && !member.Type.IsNullableType())
                throw new InvalidOperationException($"Member:{member} can not use '{ComparisonOperator.IsNull}' compare");

            var nullVal = CreateQueryParamExpression(null, member.Type);

            if (value == null || Equals(value, false))
                return Expression.Equal(member, nullVal);

            return Expression.NotEqual(member, nullVal);
        }


        private static Expression CreateHasFlagExpression(MemberExpression member, object value)
        {
            if (!member.Type.GetNonNullableType().IsEnum)
                throw new InvalidOperationException($"Member:{member} is not a Enum type");
            var list = GetListValue(member.Type.GetNonNullableType(), value);
            if (list == null || list.Count == 0) return null;

            var p = member;
            if (member.Type.IsNullableType())
                p = Expression.Property(member, "Value");

            Expression exp = null;
            foreach (var item in list)
            {
                var val = CreateQueryParamExpression(item, typeof(Enum));
                var method = typeof(Enum).GetMethod(nameof(Enum.HasFlag), new[] { typeof(Enum) });
                Expression temp = Expression.Call(p, method, val);
                exp = exp != null ? Expression.OrElse(exp, temp) : temp;
            }

            return exp;
        }

        private static IList GetListValue(Type memberType, object value)
        {
            if (value == null) return null;
            var data = value as IEnumerable;
            if (value is string str)
            {
                data = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim()).ToArray();
            }

            if (data == null)
            {
                data = new[] { value };
            }

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(memberType));

            foreach (var item in data)
            {
                try
                {
                    list.Add(ChangeType(item, memberType));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                }
            }

            return list.Count == 0 ? null : list;
        }

        private static object ChangeType(object value, Type type)
        {
            if (value == null) return null;

            type = type.GetNonNullableType();
            if (type == value.GetType().GetNonNullableType()) return value;

            if (type.IsEnum)
            {
                if (value is string str1)
                    return Enum.Parse(type, str1);
                else
                    return Enum.ToObject(type, value);
            }
            if (value is string str2 && type == typeof(Guid))
                return new Guid(str2);

            return Convert.ChangeType(value, type);
        }

        private static Expression CreateQueryParamExpression(object value, Type type)
        {
            var queryType = _createValueTupleMethod.MakeGenericMethod(type);
            var queryParam = queryType.Invoke(null, new[] { value });
            var param = Expression.Constant(queryParam);

            return Expression.Field(param, "Value");
        }

        private static readonly MethodInfo _createValueTupleMethod =
            typeof(QueryExtensions).GetMethod(nameof(CreateQueryParam), BindingFlags.Static | BindingFlags.NonPublic);
        private static QueryParam<T> CreateQueryParam<T>(T value)
        {
            return new QueryParam<T>(value);
        }

        public struct QueryParam<TValue>
        {
            public TValue Value;

            public QueryParam(TValue value)
            {
                Value = value;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (obj is QueryParam<TValue>)
                    return this.Equals((QueryParam<TValue>)obj);
                return false;
            }

            public bool Equals(QueryParam<TValue> other)
            {
                return EqualityComparer<TValue>.Default.Equals(this.Value, other.Value);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return EqualityComparer<TValue>.Default.GetHashCode(this.Value);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"QueryParam<{typeof(TValue).Name}>";
            }
        }
    }
}

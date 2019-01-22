using Castle.Core.Internal;
using Castle.DynamicProxy.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Xpress.Core.Domain.Aggregates;
using Xpress.Core.Dtos;
using Xpress.Core.Utils;

namespace Xpress.Core.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> PrettyPrintCache = new ConcurrentDictionary<Type, string>();

        public static string PrettyPrint(this Type type)
        {
            return PrettyPrintCache.GetOrAdd(
                type,
                t =>
                {
                    try
                    {
                        return PrettyPrintRecursive(t, 0);
                    }
                    catch (Exception)
                    {
                        return t.Name;
                    }
                });
        }

        private static readonly ConcurrentDictionary<Type, string> TypeCacheKeys = new ConcurrentDictionary<Type, string>();
        public static string GetCacheKey(this Type type)
        {
            return TypeCacheKeys.GetOrAdd(
                type,
                t => $"{t.PrettyPrint()}[hash: {t.GetHashCode()}]");
        }

        private static string PrettyPrintRecursive(Type type, int depth)
        {
            if (depth > 3)
            {
                return type.Name;
            }

            var nameParts = type.Name.Split('`');
            if (nameParts.Length == 1)
            {
                return nameParts[0];
            }

            var genericArguments = type.GetTypeInfo().GetGenericArguments();
            return !type.IsConstructedGenericType
                ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
                : $"{nameParts[0]}<{string.Join(",", genericArguments.Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
        }

        private static readonly ConcurrentDictionary<Type, AggregateName> AggregateNames = new ConcurrentDictionary<Type, AggregateName>();

        public static AggregateName GetAggregateName(
            this Type aggregateType)
        {
            return AggregateNames.GetOrAdd(
                aggregateType,
                t =>
                {
                    if (!typeof(IAggregateRoot).GetTypeInfo().IsAssignableFrom(aggregateType))
                    {
                        throw new ArgumentException($"Type '{aggregateType.PrettyPrint()}' is not an aggregate root");
                    }

                    return new AggregateName(
                        t.GetTypeInfo().GetCustomAttributes<AggregateNameAttribute>().SingleOrDefault()?.Name ??
                        t.Name);
                });
        }

        internal static IReadOnlyDictionary<Type, Action<T, IAggregateEvent>> GetAggregateEventApplyMethods<TAggregate, TIdentity, T>(this Type type)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregateEventType = typeof(IAggregateEvent<TAggregate, TIdentity>);

            return type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Apply") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1 &&
                        aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => ReflectionHelper.CompileMethodInvocation<Action<T, IAggregateEvent>>(type, "Apply", mi.GetParameters()[0].ParameterType));
        }

        public static bool IsSubclassOfGeneric(this Type type, Type generic)
        {
            if (type == null || generic == null)
            {
                throw new ArgumentNullException();
            }

            if (!generic.IsGenericTypeDefinition)
            {
                throw new ArgumentException(nameof(generic) + " is must be generic type definition.");
            }

            if (!generic.IsGenericType)
            {
                return false;
            }

            if (type == generic)
                return false;
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                    return true;
                type = type.BaseType;
            }

            return false;
        }
        public static Type[] GetGenericArguments(this Type type, Type generic)
        {
            if (type == null || generic == null)
            {
                throw new ArgumentNullException();
            }

            if (!generic.IsGenericTypeDefinition)
            {
                throw new ArgumentException(nameof(generic) + " is must be generic type definition.");
            }

            if (!generic.IsGenericType)
            {
                return Type.EmptyTypes;
            }

            if (type == generic)
                return Type.EmptyTypes;

            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                    return type.GetGenericArguments();
                type = type.BaseType;
            }

            return Type.EmptyTypes;
        }

        public static Type[] GetGenericInterfaceArguments(this Type type, Type generic)
        {
            if (type == null || generic == null)
            {
                throw new ArgumentNullException();
            }

            if (!generic.IsInterface)
            {
                throw new ArgumentException(nameof(generic) + " is must be interface.");
            }

            if (!generic.IsGenericTypeDefinition)
            {
                throw new ArgumentException(nameof(generic) + " is must be generic type definition.");
            }

            if (!generic.IsGenericType)
            {
                return Type.EmptyTypes;
            }

            if (type == generic)
                return Type.EmptyTypes;
            while (type != null)
            {
                Type t;
                if (type.IsGenericType && (t = type.GetInterfaces()
                        .FirstOrDefault(o => o.IsGenericType && o.GetGenericTypeDefinition() == generic)) != null)
                    return t.GetGenericArguments();
                type = type.BaseType;
            }

            return Type.EmptyTypes;
        }

       /// <summary>
        /// 获取<see cref="Nullable{TValue}"/>范型的构造类型
        /// </summary>
        public static Type GetTypeOfNullable(this Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static Type GetNonNullableType(this Type type)
        {
            if (!type.IsNullableType())
            {
                return type;
            }

            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// 判断类型是否为 集合类型 <see cref="ICollection{TValue}"/>
        /// </summary>
        public static bool IsCollectionType(this Type type)
        {
            return ((type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(ICollection<>))) ||
                    (from t in type.GetInterfaces()
                     where t.IsGenericType
                     select t.GetGenericTypeDefinition()).Any<Type>(t => (t == typeof(ICollection<>))));
        }

        /// <summary>
        /// 判断类型是否为 字典类型 <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        public static bool IsDictionaryType(this Type type)
        {
            return ((type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>))) ||
                    (from t in type.GetInterfaces()
                     where t.IsGenericType
                     select t.GetGenericTypeDefinition()).Any<Type>(t => (t == typeof(IDictionary<,>))));
        }

        /// <summary>
        /// 判断类型是否为 可迭代类型 <see cref="IEnumerable"/>
        /// </summary>
        public static bool IsEnumerableType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        /// <summary>
        /// 判断类型是否为 列表类型 <see cref="IList"/> 或 字典类型 <see cref="IDictionary{TKey, TValue}"/>
        /// </summary>
        public static bool IsListOrDictionaryType(this Type type)
        {
            if (!type.IsListType())
            {
                return type.IsDictionaryType();
            }
            return true;
        }
        /// <summary>
        /// 判断类型是否为 列表类型 <see cref="IList"/>
        /// </summary>
        public static bool IsListType(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IList));
        }

        /// <summary>
        /// 获取枚举指定的显示内容
        /// </summary>
        public static object Display(this MemberInfo memberInfo, DisplayProperty property)
        {
            if (memberInfo == null) return null;

            var display = memberInfo.GetAttribute<DisplayAttribute>();

            if (display != null)
            {
                switch (property)
                {
                    case DisplayProperty.Name:
                        return display.GetName();
                    case DisplayProperty.ShortName:
                        return display.GetShortName();
                    case DisplayProperty.GroupName:
                        return display.GetGroupName();
                    case DisplayProperty.Description:
                        return display.GetDescription();
                    case DisplayProperty.Order:
                        return display.GetOrder();
                    case DisplayProperty.Prompt:
                        return display.GetPrompt();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Name) as string;
        }

        /// <summary>
        /// 获取枚举说明
        /// </summary>
        public static string DisplayShortName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.ShortName) as string;
        }

        /// <summary>
        /// 获取枚举分组名称
        /// </summary>
        public static string DisplayGroupName(this MemberInfo val)
        {
            return val.Display(DisplayProperty.GroupName) as string;
        }

        /// <summary>
        /// 获取枚举水印信息
        /// </summary>
        public static string DisplayPrompt(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Prompt) as string;
        }

        /// <summary>
        /// 获取枚举备注
        /// </summary>
        public static string DisplayDescription(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Description) as string;
        }

        /// <summary>
        /// 获取枚举显示排序
        /// </summary>
        public static int? DisplayOrder(this MemberInfo val)
        {
            return val.Display(DisplayProperty.Order) as int?;
        }


        /// <summary>
        /// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenType.BaseType, genericType);
        }

        /// <summary>
        /// Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <param name="memberInfo">MemberInfo</param>
        public static List<object> GetAttributesOfMemberAndDeclaringType(this MemberInfo memberInfo)
        {
            var attributeList = new List<object>();

            attributeList.AddRange(memberInfo.GetCustomAttributes(true));

            //Add attributes on the class
            if (memberInfo.DeclaringType != null)
            {
                attributeList.AddRange(memberInfo.DeclaringType.GetCustomAttributes(true));
            }

            return attributeList;
        }

        /// <summary>
        /// Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        public static List<TAttribute> GetAttributesOfMemberAndDeclaringType<TAttribute>(this MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            //Add attributes on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            //Add attributes on the class
            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(memberInfo.DeclaringType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            return attributeList;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(this MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute))
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();
            }

            //Get attribute from class
            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsDefined(typeof(TAttribute), true))
            {
                return memberInfo.DeclaringType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(this MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute))
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

    }
}

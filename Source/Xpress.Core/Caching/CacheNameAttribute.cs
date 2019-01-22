using JetBrains.Annotations;
using System;
using Xpress.Core.Utils;

namespace Xpress.Core.Caching
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class CacheNameAttribute : Attribute
    {
        public string Name { get; }

        public CacheNameAttribute([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
        }
    }
}

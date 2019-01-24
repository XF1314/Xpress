using Hangfire.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpress.Core.Utils;

namespace Xpress.Core.BackgroundJobs
{
    public class BackgroundJobNameAttribute : Attribute, IBackgroundJobNameProvider
    {
        public string Name { get; }

        public BackgroundJobNameAttribute([NotNull] string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        }

        public static string GetName<TArgs>() where TArgs : IBackgroundEventArgs
        {
            return GetName(typeof(TArgs));
        }

        public static string GetName([NotNull] Type jobArgsType)
        {
            Check.NotNull(jobArgsType, nameof(jobArgsType));

            return jobArgsType
                       .GetCustomAttributes(true)
                       .OfType<IBackgroundJobNameProvider>()
                       .FirstOrDefault()
                       ?.Name
                   ?? jobArgsType.FullName;
        }
    }
}

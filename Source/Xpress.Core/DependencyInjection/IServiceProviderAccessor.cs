using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.DependencyInjection
{
    public interface IServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}

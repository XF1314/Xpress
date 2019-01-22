using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace Xpress.Core.DependencyInjection
{
    /// <summary>
    /// Base class of interceptor
    /// </summary>
    public abstract class InterceptorBase : IInterceptor
    {
        /// <summary>
        /// Intercept execution method
        /// </summary>
        public abstract void Intercept(IInvocation invocation);
    }
}


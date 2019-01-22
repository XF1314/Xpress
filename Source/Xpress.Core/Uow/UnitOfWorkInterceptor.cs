using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Threading;

namespace Xpress.Core.Uow
{
    /// <summary>
    /// Unit of work interceptor
    /// </summary>
    internal class UnitOfWorkInterceptor : InterceptorBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWorkOptions _defaultUnitOfWorkOptions;

        public UnitOfWorkInterceptor(DefaultUnitOfWorkOptions unitOfWorkOptions, IUnitOfWorkManager unitOfWorkManager)
        {
            _defaultUnitOfWorkOptions = unitOfWorkOptions.Clone();
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo;
            try
            {
                methodInfo = invocation.MethodInvocationTarget;
            }
            catch
            {
                methodInfo = invocation.GetConcreteMethod();
            }

            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
                invocation.Proceed();
            else
            {
                using (var unitOfWork = _unitOfWorkManager.Begin(CreateOptions(invocation, unitOfWorkAttribute)))
                {
                    invocation.Proceed();
                    if (!invocation.Method.IsAsync())
                        unitOfWork.Complete();
                    else
                    {
                        if (invocation.Method.ReturnType == typeof(Task))
                        {
                            invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                                (Task)invocation.ReturnValue,
                                async () =>
                                 {
                                     await unitOfWork.CompleteAsync();
                                     await Task.FromResult(0);
                                 },
                                exception => unitOfWork.Dispose()
                            );
                        }
                        else //Task<TResult>
                        {
                            invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                                invocation.Method.ReturnType.GenericTypeArguments[0],
                                invocation.ReturnValue,
                                async () =>
                                {
                                    await unitOfWork.CompleteAsync();
                                    await Task.FromResult(0);
                                },
                                exception => unitOfWork.Dispose()
                            );
                        }
                    }
                }
            }
        }

        private UnitOfWorkOptions CreateOptions(IInvocation invocation, [CanBeNull] UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var unitOfWorkOptions = _defaultUnitOfWorkOptions.Clone();
            unitOfWorkAttribute?.SetOptions(unitOfWorkOptions);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                unitOfWorkOptions.IsTransactional = !invocation.Method.Name.StartsWith("Get", StringComparison.InvariantCultureIgnoreCase);
            }

            return unitOfWorkOptions;
        }
    }
}

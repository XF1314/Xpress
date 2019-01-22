using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xpress.AspNetCore.Extensions;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Uow;

namespace Xpress.AspNetCore.Uow
{
    public class UowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWorkOptions _defaultUnitOfWorkOptions;

        public UowActionFilter(IUnitOfWorkManager unitOfWorkManager, DefaultUnitOfWorkOptions defaultUnitOfWorkOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _defaultUnitOfWorkOptions = defaultUnitOfWorkOptions.Clone();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionDescriptor.IsControllerAction())
                await next();
            else
            {
                var methodInfo = context.ActionDescriptor.GetMethodInfo();
                var unitOfWorkAttribute = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);
                if (unitOfWorkAttribute?.IsDisabled == true)
                    await next();
                else//Trying to begin a reserved UOW by UnitOfWorkMiddleware
                {
                    var unitOfWorkOptions = CreateOptions(context, unitOfWorkAttribute);
                    if (_unitOfWorkManager.TryBeginReserved(UnitOfWorkMiddleware.UnitOfWorkReservationName, unitOfWorkOptions))
                    {
                        var result = await next();
                        if (!Succeed(result))
                        {
                            Rollback();
                        }

                        return;
                    }
                    else //Begin a new, independent unit of work
                    {
                        using (var uow = _unitOfWorkManager.Begin(unitOfWorkOptions))
                        {
                            var result = await next();
                            if (Succeed(result))
                            {
                                await uow.CompleteAsync(context.HttpContext.RequestAborted);
                            }
                        }
                    }
                }
            }
        }

        private UnitOfWorkOptions CreateOptions(ActionExecutingContext context, UnitOfWorkAttribute unitOfWorkAttribute)
        {
            var unitOfWorkOptions = _defaultUnitOfWorkOptions.Clone();
            unitOfWorkAttribute?.SetOptions(unitOfWorkOptions);

            if (unitOfWorkAttribute?.IsTransactional == null)
            {
                unitOfWorkOptions.IsTransactional = !string.Equals(context.HttpContext.Request.Method, HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase);
            }

            return unitOfWorkOptions;
        }

        private void Rollback()
        {
            var currentUow = _unitOfWorkManager.Current;
            if (currentUow != null)
            {
                currentUow.Rollback();
            }
        }

        private static bool Succeed(ActionExecutedContext result)
        {
            return result.Exception == null || result.ExceptionHandled;
        }
    }
}
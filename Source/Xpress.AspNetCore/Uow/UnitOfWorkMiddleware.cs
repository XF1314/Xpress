using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.Core.Uow;

namespace Xpress.AspNetCore.Uow
{
    public class UnitOfWorkMiddleware
    {
        public const string UnitOfWorkReservationName = "_ActionUnitOfWork";

        private readonly RequestDelegate _next;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkMiddleware(RequestDelegate next, IUnitOfWorkManager unitOfWorkManager)
        {
            _next = next;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (var uow = _unitOfWorkManager.Reserve(UnitOfWorkReservationName))
            {
                await _next(httpContext);
                await uow.CompleteAsync(httpContext.RequestAborted);
            }
        }
    }
}

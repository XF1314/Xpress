using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xpress.Core;
using Xpress.Core.Application;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;

namespace Xpress.AspNetCore.ExceptionHandling
{
    public class DefaultHttpExceptionStatusCodeFinder : IHttpExceptionStatusCodeFinder, ITransientDependency
    {
        protected ExceptionHttpStatusCodeOptions Options { get; }

        public DefaultHttpExceptionStatusCodeFinder(
            IOptions<ExceptionHttpStatusCodeOptions> options)
        {
            Options = options.Value;
        }

        public virtual HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
        {
            if (exception is IHasErrorCode exceptionWithErrorCode &&
                !exceptionWithErrorCode.Code.IsNullOrWhiteSpace())
            {
                if (Options.ErrorCodeToHttpStatusCodeMappings.TryGetValue(exceptionWithErrorCode.Code, out var status))
                {
                    return status;
                }
            }

            if (exception is XpressAuthorizationException)
            {
                return httpContext.User.Identity.IsAuthenticated
                    ? HttpStatusCode.Forbidden
                    : HttpStatusCode.Unauthorized;
            }

            //TODO: Handle SecurityException..?

            if (exception is XpressValidationException)
            {
                return HttpStatusCode.BadRequest;
            }

            if (exception is EntityNotFoundException)
            {
                return HttpStatusCode.NotFound;
            }

            if (exception is NotImplementedException)
            {
                return HttpStatusCode.NotImplemented;
            }

            if (exception is IBusinessException)
            {
                return HttpStatusCode.Forbidden;
            }

            return HttpStatusCode.InternalServerError;
        }
    }
}

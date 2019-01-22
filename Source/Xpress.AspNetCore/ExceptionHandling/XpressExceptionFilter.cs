using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.AspNetCore.Extensions;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;
using Xpress.Core.Http;
using Xpress.Core.Serialization;

namespace Xpress.AspNetCore.ExceptionHandling
{
    public class XpressExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger<XpressExceptionFilter> Logger { get; set; }

        private readonly IExceptionToErrorInfoConverter _errorInfoConverter;
        private readonly IHttpExceptionStatusCodeFinder _statusCodeFinder;
        private readonly IJsonSerializer _jsonSerializer;

        public XpressExceptionFilter(
            IExceptionToErrorInfoConverter errorInfoConverter,
            IHttpExceptionStatusCodeFinder statusCodeFinder,
            IJsonSerializer jsonSerializer)
        {
            _errorInfoConverter = errorInfoConverter;
            _statusCodeFinder = statusCodeFinder;
            _jsonSerializer = jsonSerializer;

            Logger = NullLogger<XpressExceptionFilter>.Instance;
        }

        public virtual void OnException(ExceptionContext context)
        {
            if (!ShouldHandleException(context))
                return;
            else
            {
                HandleAndWrapException(context);
            }
        }

        protected virtual bool ShouldHandleException(ExceptionContext context)
        {
            //TODO: Create DontWrap attribute to control wrapping..?
            if (context.ActionDescriptor.IsControllerAction() && context.ActionDescriptor.HasObjectResult())
                return true;
            else if (context.HttpContext.Request.CanAccept(MimeTypes.Application.Json))
                return true;
            else if (context.HttpContext.Request.IsAjax())
                return true;
            else
            {
                return false;
            }
        }

        protected virtual void HandleAndWrapException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = (int)_statusCodeFinder.GetStatusCode(context.HttpContext, context.Exception);
            var remoteServiceErrorInfo = _errorInfoConverter.Convert(context.Exception);

            context.Result = new ObjectResult(new RemoteServiceErrorResponse(remoteServiceErrorInfo));

            var logLevel = context.Exception.GetLogLevel();
            Logger.LogWithLevel(logLevel, $"---------- {nameof(RemoteServiceErrorInfo)} ----------");
            Logger.LogWithLevel(logLevel, _jsonSerializer.Serialize(remoteServiceErrorInfo, indented: true));
            Logger.LogException(context.Exception, logLevel);

            context.Exception = null; //Handled!
        }
    }
}

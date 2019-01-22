using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpress.Core;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;
using Xpress.Core.Extensions;
using Xpress.Core.Http;

namespace Xpress.AspNetCore.ExceptionHandling
{
    public class DefaultExceptionToErrorInfoConverter : IExceptionToErrorInfoConverter, ITransientDependency
    {
        public bool SendAllExceptionsToClients { get; set; } = false;
        protected IServiceProvider ServiceProvider { get; }

        public DefaultExceptionToErrorInfoConverter(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public RemoteServiceErrorInfo Convert(Exception exception)
        {
            var errorInfo = CreateErrorInfoWithoutCode(exception);
            if (exception is IHasErrorCode)
            {
                errorInfo.Code = (exception as IHasErrorCode).Code;
            }

            return errorInfo;
        }

        protected virtual RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception)
        {
            if (SendAllExceptionsToClients)
                return CreateDetailedErrorInfoFromException(exception);
            else
            {
                exception = TryToGetActualException(exception);
                if (exception is EntityNotFoundException)
                    return CreateEntityNotFoundError(exception as EntityNotFoundException);
                else if (exception is XpressAuthorizationException)
                {
                    var authorizationException = exception as XpressAuthorizationException;
                    return new RemoteServiceErrorInfo(authorizationException.Message);
                }
                else
                {
                    var errorInfo = new RemoteServiceErrorInfo();
                    if (exception is IHasValidationErrors)
                    {
                        if (errorInfo.Details.IsNullOrEmpty())
                        {
                            errorInfo.Details = GetValidationErrorNarrative(exception as IHasValidationErrors);
                        }

                        errorInfo.ValidationErrors = GetValidationErrorInfos(exception as IHasValidationErrors);
                    }

                    if (exception is IUserFriendlyException)
                    {
                        errorInfo.Message = exception.Message;
                        errorInfo.Details = (exception as IHasErrorDetails)?.Details;
                    }
                    else
                    {
                        errorInfo.Message = "远程服务异常";
                        errorInfo.Details = exception.Message;
                    }

                    return errorInfo;
                }
            }
        }

        protected virtual RemoteServiceErrorInfo CreateEntityNotFoundError(EntityNotFoundException exception)
        {
            if (exception.EntityType != null)
            {
                return new RemoteServiceErrorInfo(
                    string.Format(
                        "无对应Entity",
                        exception.EntityType.Name,
                        exception.Id
                    )
                );
            }

            return new RemoteServiceErrorInfo(exception.Message);
        }

        protected virtual Exception TryToGetActualException(Exception exception)
        {
            if (exception is AggregateException && exception.InnerException != null)
            {
                var aggException = exception as AggregateException;

                if (aggException.InnerException is IUserFriendlyException ||
                    aggException.InnerException is XpressValidationException ||
                    aggException.InnerException is EntityNotFoundException ||
                    aggException.InnerException is XpressAuthorizationException ||
                    aggException.InnerException is IBusinessException)
                {
                    return aggException.InnerException;
                }
            }

            return exception;
        }


        protected virtual RemoteServiceErrorInfo CreateDetailedErrorInfoFromException(Exception exception)
        {
            var detailBuilder = new StringBuilder();
            AddExceptionToDetails(exception, detailBuilder);

            var errorInfo = new RemoteServiceErrorInfo(exception.Message, detailBuilder.ToString());
            if (exception is XpressValidationException)
            {
                errorInfo.ValidationErrors = GetValidationErrorInfos(exception as XpressValidationException);
            }

            return errorInfo;
        }

        protected virtual void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
        {
            //Exception Message
            detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

            //Additional info for UserFriendlyException
            if (exception is IUserFriendlyException &&
                exception is IHasErrorDetails)
            {
                var details = ((IHasErrorDetails)exception).Details;
                if (!details.IsNullOrEmpty())
                {
                    detailBuilder.AppendLine(details);
                }
            }

            //Additional info for ValidationException
            if (exception is XpressValidationException)
            {
                var validationException = exception as XpressValidationException;
                if (validationException.ValidationErrors.Count > 0)
                {
                    detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
                }
            }

            //Exception StackTrace
            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
            }

            //Inner exception
            if (exception.InnerException != null)
            {
                AddExceptionToDetails(exception.InnerException, detailBuilder);
            }

            //Inner exceptions for AggregateException
            if (exception is AggregateException)
            {
                var aggException = exception as AggregateException;
                if (aggException.InnerExceptions.IsNullOrEmpty())
                {
                    return;
                }

                foreach (var innerException in aggException.InnerExceptions)
                {
                    AddExceptionToDetails(innerException, detailBuilder);
                }
            }
        }

        protected virtual RemoteServiceValidationErrorInfo[] GetValidationErrorInfos(IHasValidationErrors validationException)
        {
            var validationErrorInfos = new List<RemoteServiceValidationErrorInfo>();

            foreach (var validationResult in validationException.ValidationErrors)
            {
                var validationError = new RemoteServiceValidationErrorInfo(validationResult.ErrorMessage);

                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                {
                    validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
                }

                validationErrorInfos.Add(validationError);
            }

            return validationErrorInfos.ToArray();
        }

        protected virtual string GetValidationErrorNarrative(IHasValidationErrors validationException)
        {
            var detailBuilder = new StringBuilder();
            foreach (var validationResult in validationException.ValidationErrors)
            {
                detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
                detailBuilder.AppendLine();
            }

            return detailBuilder.ToString();
        }
    }
}

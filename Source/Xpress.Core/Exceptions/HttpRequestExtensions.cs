using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Utils;

namespace Xpress.Core.Exceptions
{
    public static class HttpRequestExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        public static bool IsAjax([NotNull]this HttpRequest request)
        {
            Check.NotNull(request, nameof(request));

            if (request.Headers == null)
            {
                return false;
            }

            return request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }

        public static bool CanAccept([NotNull]this HttpRequest request, [NotNull] string contentType)
        {
            Check.NotNull(request, nameof(request));
            Check.NotNull(contentType, nameof(contentType));

            return request.Headers["Accept"].ToString().Contains(contentType);
        }
    }
}

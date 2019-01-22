using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Xpress.AspNetCore.ExceptionHandling
{
    public interface IHttpExceptionStatusCodeFinder
    {
        HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception);
    }
}

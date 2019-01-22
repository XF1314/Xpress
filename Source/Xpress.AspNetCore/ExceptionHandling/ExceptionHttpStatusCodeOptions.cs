using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Xpress.AspNetCore.ExceptionHandling
{
    public class ExceptionHttpStatusCodeOptions
    {
        public IDictionary<string, HttpStatusCode> ErrorCodeToHttpStatusCodeMappings { get; }

        public ExceptionHttpStatusCodeOptions()
        {
            ErrorCodeToHttpStatusCodeMappings = new Dictionary<string, HttpStatusCode>();
        }

        public void Map(string errorCode, HttpStatusCode httpStatusCode)
        {
            ErrorCodeToHttpStatusCodeMappings[errorCode] = httpStatusCode;
        }
    }
}

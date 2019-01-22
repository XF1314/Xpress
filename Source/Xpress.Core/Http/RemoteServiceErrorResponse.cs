using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.Core.Http
{
    public class RemoteServiceErrorResponse
    {
        public RemoteServiceErrorInfo Error { get; set; }

        public RemoteServiceErrorResponse(RemoteServiceErrorInfo error)
        {
            Error = error;
        }
    }
}

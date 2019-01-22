using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xpress.Core.Threading;
using Xpress.Core;
using Xpress.Core.Utils;

namespace Xpress.AspNetCore.Utils
{
    public static class ActionResultHelper
    {
        public static List<Type> ObjectResultTypes { get; }

        static ActionResultHelper()
        {
            ObjectResultTypes = new List<Type>
            {
                typeof(JsonResult),
                typeof(ObjectResult),
                typeof(NoContentResult)
            };
        }

        public static bool IsObjectResult(Type returnType)
        {
            returnType = XpressAsyncHelper.UnwrapTask(returnType);

            if (!typeof(IActionResult).IsAssignableFrom(returnType))
            {
                return true;
            }

            return ObjectResultTypes.Any(t => t.IsAssignableFrom(returnType));
        }
    }
}

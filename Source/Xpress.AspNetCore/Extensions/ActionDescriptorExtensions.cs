using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xpress.AspNetCore.Utils;
using Xpress.Core.Exceptions;

namespace Xpress.AspNetCore.Extensions
{
    public static class ActionDescriptorExtensions
    {
        public static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
        {
            if (!actionDescriptor.IsControllerAction())
            {
                throw new XpressException($"{nameof(actionDescriptor)} should be type of {typeof(ControllerActionDescriptor).AssemblyQualifiedName}");
            }

            return actionDescriptor as ControllerActionDescriptor;
        }

        public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
        }

        public static Type GetReturnType(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetMethodInfo().ReturnType;
        }

        public static bool HasObjectResult(this ActionDescriptor actionDescriptor)
        {
            return ActionResultHelper.IsObjectResult(actionDescriptor.GetReturnType());
        }

        public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
        {
            return actionDescriptor is ControllerActionDescriptor;
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xpress.AspNetCore.Extensions;
using Xpress.Core.DependencyInjection;

namespace Xpress.AspNetCore.ModelValidation
{
    public class XpressModelValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IModelStateValidator _validator;

        public XpressModelValidationActionFilter(IModelStateValidator validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //TODO: Configuration to disable validation for controllers..?

            if (context.ActionDescriptor.IsControllerAction() && context.ActionDescriptor.HasObjectResult())
            {
                _validator.Validate(context.ModelState);
            }

            await next();
        }
    }
}

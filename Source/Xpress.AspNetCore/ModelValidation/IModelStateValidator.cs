using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xpress.AspNetCore.ModelValidation
{
    public interface IModelStateValidator
    {
        void Validate(ModelStateDictionary modelState);

        void AddErrors(IXpressValidationResult validationResult, ModelStateDictionary modelState);
    }
}

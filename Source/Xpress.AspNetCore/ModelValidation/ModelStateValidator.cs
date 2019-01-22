using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Exceptions;

namespace Xpress.AspNetCore.ModelValidation
{
    public class ModelStateValidator : IModelStateValidator, ITransientDependency
    {
        public virtual void Validate(ModelStateDictionary modelState)
        {
            var validationResult = new XpressValidationResult();

            AddErrors(validationResult, modelState);

            if (validationResult.Errors.Any())
            {
                throw new XpressValidationException(
                    "ModelState 验证未通过，具体原因看详情。",
                    validationResult.Errors
                );
            }
        }

        public virtual void AddErrors(IXpressValidationResult validationResult, ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                return;
            }

            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    validationResult.Errors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }
        }
    }
}

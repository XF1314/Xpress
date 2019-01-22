using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpress.AspNetCore.ModelValidation
{
    public interface IXpressValidationResult
    {
        List<ValidationResult> Errors { get; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpress.AspNetCore.ModelValidation
{
    public class XpressValidationResult : IXpressValidationResult
    {
        public List<ValidationResult> Errors { get; }

        public XpressValidationResult()
        {
            Errors = new List<ValidationResult>();
        }
    }
}

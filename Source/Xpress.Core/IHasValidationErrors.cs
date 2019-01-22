using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xpress.Core
{
    public interface IHasValidationErrors
    {
        IList<ValidationResult> ValidationErrors { get; }
    }
}

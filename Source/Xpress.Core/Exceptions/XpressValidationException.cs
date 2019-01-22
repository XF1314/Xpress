using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Xpress.Core.Exceptions
{
    /// <summary>
    /// This exception type is used to throws validation exceptions.
    /// </summary>
    [Serializable]
    public class XpressValidationException : XpressException, IHasValidationErrors, IHasLogLevel
    {
        /// <summary>
        /// Detailed list of validation errors for this exception.
        /// </summary>
        public IList<ValidationResult> ValidationErrors { get; }

        /// <summary>
        /// Exception severity.
        /// Default: Warn.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public XpressValidationException()
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public XpressValidationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        public XpressValidationException(string message)
            : base(message)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="validationErrors">Validation errors</param>
        public XpressValidationException(string message, IList<ValidationResult> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public XpressValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = new List<ValidationResult>();
            LogLevel = LogLevel.Warning;
        }
    }
}

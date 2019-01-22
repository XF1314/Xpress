using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Xpress.Core.Exceptions
{
    /// <summary>
    /// This exception is thrown on an unauthorized request.
    /// </summary>
    [Serializable]
    public class XpressAuthorizationException : XpressException, IHasLogLevel
    {
        /// <summary>
        /// Severity of the exception.
        /// Default: Warn.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Creates a new <see cref="XpressAuthorizationException"/> object.
        /// </summary>
        public XpressAuthorizationException()
        {
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Creates a new <see cref="XpressAuthorizationException"/> object.
        /// </summary>
        public XpressAuthorizationException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="XpressAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public XpressAuthorizationException(string message)
            : base(message)
        {
            LogLevel = LogLevel.Warning;
        }

        /// <summary>
        /// Creates a new <see cref="XpressAuthorizationException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public XpressAuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        {
            LogLevel = LogLevel.Warning;
        }
    }
}

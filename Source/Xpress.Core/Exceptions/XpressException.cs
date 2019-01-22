using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Xpress.Core.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Abp system for Abp specific exceptions.
    /// </summary>
    public class XpressException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="XpressException"/> object.
        /// </summary>
        public XpressException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="XpressException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public XpressException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="XpressException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public XpressException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public XpressException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }
    }
}

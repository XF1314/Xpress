using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Xpress.Core.Exceptions;

namespace Xpress.Core.BackgroundJobs
{
    [Serializable]
    public class BackgroundJobExecutionException : XpressException
    {
        public string JobType { get; set; }

        public object JobArgs { get; set; }

        public BackgroundJobExecutionException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="BackgroundJobExecutionException"/> object.
        /// </summary>
        public BackgroundJobExecutionException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="BackgroundJobExecutionException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public BackgroundJobExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}

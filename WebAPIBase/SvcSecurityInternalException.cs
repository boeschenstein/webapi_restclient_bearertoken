using System;
using System.Runtime.Serialization;

namespace WebAPIBase
{
    [Serializable]
    internal class SvcSecurityInternalException : Exception
    {
        public SvcSecurityInternalException()
        {
        }

        public SvcSecurityInternalException(string message) : base(message)
        {
        }

        public SvcSecurityInternalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SvcSecurityInternalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
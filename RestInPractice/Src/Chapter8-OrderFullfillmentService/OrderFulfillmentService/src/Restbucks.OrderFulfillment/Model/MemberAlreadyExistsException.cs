using System;
using System.Runtime.Serialization;

namespace Restbucks.OrderFulfillment.Model
{
    public class MemberAlreadyExistsException : Exception
    {
        public MemberAlreadyExistsException()
        {
        }

        public MemberAlreadyExistsException(string message) : base(message)
        {
        }

        public MemberAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MemberAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
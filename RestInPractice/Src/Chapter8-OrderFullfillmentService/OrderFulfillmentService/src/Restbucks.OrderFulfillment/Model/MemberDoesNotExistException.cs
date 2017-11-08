using System;
using System.Runtime.Serialization;

namespace Restbucks.OrderFulfillment.Model
{
    public class MemberDoesNotExistException : Exception
    {
        public MemberDoesNotExistException()
        {
        }

        public MemberDoesNotExistException(string message) : base(message)
        {
        }

        public MemberDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MemberDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace Restbucks.OrderFulfillment.Model
{
    public class MemberNoLongerExistsException : Exception
    {
        public MemberNoLongerExistsException()
        {
        }

        public MemberNoLongerExistsException(string message) : base(message)
        {
        }

        public MemberNoLongerExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MemberNoLongerExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
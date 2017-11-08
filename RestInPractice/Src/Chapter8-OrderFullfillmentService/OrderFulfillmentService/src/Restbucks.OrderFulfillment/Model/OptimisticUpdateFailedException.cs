using System;
using System.Runtime.Serialization;

namespace Restbucks.OrderFulfillment.Model
{
    public class OptimisticUpdateFailedException : Exception
    {
        public OptimisticUpdateFailedException()
        {
        }

        public OptimisticUpdateFailedException(string message) : base(message)
        {
        }

        public OptimisticUpdateFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OptimisticUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
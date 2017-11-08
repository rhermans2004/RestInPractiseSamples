using System;

namespace Restbucks.OrderFulfillment.Model
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now
        {
            get { return DateTime.Now; }
        }
    }
}
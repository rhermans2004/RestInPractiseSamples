using System;
using Restbucks.OrderFulfillment.Model;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public static IDateTimeProvider ForNow()
        {
            return new FakeDateTimeProvider(DateTime.Now);
        }

        public static IDateTimeProvider For(DateTimeOffset value)
        {
            return new FakeDateTimeProvider(value);
        }

        private readonly DateTimeOffset now;

        private FakeDateTimeProvider(DateTimeOffset now)
        {
            this.now = now;
        }

        public DateTimeOffset Now
        {
            get { return now; }
        }
    }
}
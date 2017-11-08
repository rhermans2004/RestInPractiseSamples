using System;

namespace Restbucks.OrderFulfillment.Model
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now { get; }
    }
}
using System;

namespace Restbucks.OrderFulfillment.Model
{
    public interface IIdGenerator
    {
        Guid NewId();
    }
}
using System;

namespace Restbucks.OrderFulfillment.Model
{
    public class IdGenerator : IIdGenerator
    {
        public Guid NewId()
        {
            return Guid.NewGuid();
        }
    }
}
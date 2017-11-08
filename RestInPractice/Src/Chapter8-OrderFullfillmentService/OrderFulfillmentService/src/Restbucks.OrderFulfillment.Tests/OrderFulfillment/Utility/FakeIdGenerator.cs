using System;
using Restbucks.OrderFulfillment.Model;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class FakeIdGenerator : IIdGenerator
    {
        public static IIdGenerator ForAnyId()
        {
            return new FakeIdGenerator(Guid.NewGuid());
        }

        public static IIdGenerator For(Guid value)
        {
            return new FakeIdGenerator(value);
        }

        private readonly Guid id;

        private FakeIdGenerator(Guid id)
        {
            this.id = id;
        }

        public Guid NewId()
        {
            return id;
        }
    }
}
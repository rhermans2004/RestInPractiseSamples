using System;
using System.Collections.Generic;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Model
{
    public interface IRepository
    {
        IEnumerable<Fulfillment> GetAll();
        ETaggedEntity Get(Guid id);
        ETaggedEntity Add(Fulfillment member);
        void Update(Fulfillment member, EntityTag entityTag);
        void Remove(Guid id);
    }
}
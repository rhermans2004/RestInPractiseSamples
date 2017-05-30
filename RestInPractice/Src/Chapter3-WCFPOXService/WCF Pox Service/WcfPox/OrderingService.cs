using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfPox
{
    public class OrderingService : IOrderingService
    {
        public OrderConfirmation PlaceOrder(Order order)
        {
//            string response = "";
//
//            foreach(Item id in order.ItemIds)
//            {
//                response += id.Id;
//            }
//
            return new OrderConfirmation {orderId = Guid.NewGuid().ToString()};
        }
    }
}

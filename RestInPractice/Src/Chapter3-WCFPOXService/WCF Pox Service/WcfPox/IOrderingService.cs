using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace WcfPox
{
    [ServiceContract(Namespace = "http://ordering.example.org")]
    public interface IOrderingService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate="/PlaceOrder")]
        OrderConfirmation PlaceOrder(Order order);
    }

    [DataContract(Namespace = "http://ordering.example.org/order")]
    public class Order
    {
        [DataMember] public string CustomerId;
        [DataMember] public List<Item> Items;
    }

    [DataContract(Namespace = "http://ordering.example.org/order")]
    public class Item
    {
        [DataMember] public string ItemId;
        [DataMember] public string Quantity;
    }

    [DataContract(Namespace="http://ordering.example.org/order")]
    public class OrderConfirmation
    {
        [DataMember] public string orderId;
    }
}

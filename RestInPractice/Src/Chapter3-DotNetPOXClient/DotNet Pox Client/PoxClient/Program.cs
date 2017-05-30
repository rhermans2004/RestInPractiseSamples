using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using PoxClient.ServiceReference2;

namespace PoxClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string baseAddress = "http://" + Environment.MachineName + ":7070/";


            var factory = new ChannelFactory<IOrderingService>(new WebHttpBinding(), new EndpointAddress(baseAddress));

            factory.Endpoint.Behaviors.Add(new WebHttpBehavior());

            IOrderingService client = factory.CreateChannel();

            var order = new Order { CustomerId = "1234", Items = new Item[] { new Item() {ItemId = "abc", Quantity = "101"}, new Item() {ItemId = "def", Quantity="202"}} };
            OrderConfirmation response = client.PlaceOrder(order);

            Console.WriteLine(response.orderId);


            ((IClientChannel) client).Close();

            factory.Close();

            Console.WriteLine("And again... not using the service reference...");
            Console.WriteLine(new Program().PlaceOrder("abcd", new Item[] {new Item() {ItemId = "123", Quantity = "44"}, new Item() {ItemId = "456", Quantity = "55"}}).orderId);
        }

    public OrderConfirmation PlaceOrder(string customerId, Item[] items)
    {
        string itemsInXml = string.Empty;

        foreach (Item i in items)
        {
            itemsInXml += "<Item><ItemId>" + i.ItemId + "</ItemId><Quantity>" + i.Quantity + "</Quantity></Item>";
        }


        var client = new WebClient();

        var request = new XmlDocument();
        request.LoadXml("<Order xmlns=\"http://ordering.example.org/order\">" +
                        "<CustomerId>" + customerId + "</CustomerId><Items>" +
                        itemsInXml +
                        "</Items></Order>");

        var ms = new MemoryStream();
        request.Save(ms);

        client.Headers.Add("Content-Type", "text/xml");

        ms =
            new MemoryStream(client.UploadData("http://" + Environment.MachineName + ":7070/PlaceOrder", null,
                                               ms.ToArray()));

        var response = new XmlDocument();
        response.Load(ms);

        XmlNodeList elements = response.GetElementsByTagName("orderId");

        if (elements.Count == 1)
        {
            return new OrderConfirmation {orderId = elements[0].FirstChild.Value};
        }
        else
        {
            throw new ProtocolViolationException("Unexpected response from service.");
        }
    }
}
}
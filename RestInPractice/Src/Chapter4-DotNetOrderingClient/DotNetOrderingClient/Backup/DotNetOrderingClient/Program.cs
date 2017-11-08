using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Restbucks;

namespace DotNetOrderingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Order order = new Order();
            new Program().UpdateOrder(order);
        }

        public void UpdateOrder(Order order)
        {
            HttpWebRequest request = WebRequest.Create("http://127.0.0.1.:8080/OrderingService.svc/orders/1234") as HttpWebRequest;
            request.Method = "PUT";
            request.ContentType = "application/xml";
            request.Proxy = new WebProxy("http://localhost:8888");
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Order));
            xmlSerializer.Serialize(request.GetRequestStream(), order);

            request.GetRequestStream().Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Compensation logic omitted for brevity
            }
        }
    }
}

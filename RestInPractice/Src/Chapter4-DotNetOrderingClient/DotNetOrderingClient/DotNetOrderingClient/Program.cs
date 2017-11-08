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
            HttpWebRequest request = WebRequest.Create("http://localhost:8080/OrderingService.svc/orders/1234") as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/xml";
            request.Proxy = new WebProxy("http://localhost:8080");
            
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(Order));
            //xmlSerializer.Serialize(request.GetRequestStream(), order);

            //request.GetRequestStream().Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine(response.StatusDescription);
   
            Stream dataStream = response.GetResponseStream();


            StreamReader sr = new StreamReader(dataStream);
            var content = sr.ReadToEnd();
  

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Order));
            //xmlSerializer.CanDeserialize(reader);
            object xmlOrder = xmlSerializer.Deserialize(dataStream);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Compensation logic omitted for brevity
            }
        }
    }
}

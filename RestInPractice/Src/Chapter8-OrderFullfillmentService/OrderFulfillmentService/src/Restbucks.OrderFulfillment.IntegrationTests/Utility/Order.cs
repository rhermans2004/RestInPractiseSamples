using System.Runtime.Serialization;
using System.ServiceModel.Syndication;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    [DataContract(Name = "order", Namespace = "http://schemas.restbucks.com/order")]
    public class Order
    {
        public static Order FromSyndicationContent(SyndicationContent syndicationContent)
        {
            return ((XmlSyndicationContent) syndicationContent).ReadContent<Order>();
        }

        [DataMember]
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
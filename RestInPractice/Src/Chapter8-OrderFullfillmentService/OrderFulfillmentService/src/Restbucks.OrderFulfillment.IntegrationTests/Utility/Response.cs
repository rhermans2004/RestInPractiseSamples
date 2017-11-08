using System.Xml.Linq;

namespace Restbucks.OrderFulfillment.IntegrationTests.Utility
{
    public class Response
    {
        private readonly int statusCode;
        private readonly string statusDescription;
        private readonly string contentType;
        private readonly string eTag;
        private readonly XDocument entityBody;

        public Response(int statusCode, string statusDescription, string contentType, string eTag, XDocument entityBody)
        {
            this.statusCode = statusCode;
            this.statusDescription = statusDescription;
            this.contentType = contentType;
            this.eTag = eTag;
            this.entityBody = entityBody;
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public string StatusDescription
        {
            get { return statusDescription; }
        }

        public string ContentType
        {
            get { return contentType; }
        }

        public string ETag
        {
            get { return eTag; }
        }

        public XDocument EntityBody
        {
            get { return entityBody; }
        }
    }
}
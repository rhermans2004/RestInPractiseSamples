using System;
using Restbucks.OrderFulfillment.Model;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class FulfillmentCollectionBuilder
    {
        private DateTimeOffset createdDateTime;
        private Uri baseUri;

        public FulfillmentCollectionBuilder()
        {
            createdDateTime = DateTime.Now;
            baseUri = new Uri("http://localhost/fulfillment");
        }

        public FulfillmentCollectionBuilder WithCreatedDateTime(DateTimeOffset value)
        {
            createdDateTime = value;
            return this;
        }

        public FulfillmentCollectionBuilder WithBaseUri(Uri value)
        {
            baseUri = value;
            return this;
        }

        public FulfillmentCollection Build()
        {
            return new FulfillmentCollection(createdDateTime, baseUri);
        }
    }
}
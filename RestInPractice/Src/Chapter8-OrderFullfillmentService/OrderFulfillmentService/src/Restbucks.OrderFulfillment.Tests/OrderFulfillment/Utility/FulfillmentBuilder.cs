using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit.Syndication.AtomPub;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class FulfillmentBuilder
    {
        public static Fulfillment NewFulfillment()
        {
            return new FulfillmentBuilder().Build();
        }

        public static IEnumerable<Fulfillment> BuildEditedFulfillmentList(DateTimeOffset createdDateTime, params DateTimeOffset[] editedDateTimes)
        {
            return (from dtm in editedDateTimes select BuildEditedFulfillment(createdDateTime, dtm));
        }

        public static Fulfillment BuildEditedFulfillment(DateTimeOffset createdDateTime, DateTimeOffset editedDateTime)
        {
            Fulfillment fulfillment = new FulfillmentBuilder().WithCreatedDateTime(createdDateTime).Build();
            fulfillment.EditedDateTime = editedDateTime;
            return fulfillment;
        }

        public static Fulfillment BuildNonDraftFulfillment(DateTimeOffset editedDateTime)
        {
            Fulfillment fulfillment = NewFulfillment();

            return fulfillment.Edit(new Member {Draft = DraftStatus.No}, editedDateTime);
        }

        private Guid id;
        private DateTimeOffset createdDateTime;
        private Uri baseUri;
        private XmlSyndicationContent order;
        private string agent;

        public FulfillmentBuilder()
        {
            id = Guid.NewGuid();
            createdDateTime = DateTime.Now;
            baseUri = new Uri("http://localhost/fulfillment");
            order = new XmlSyndicationContent(MediaTypes.Restbucks.TypeAndSubtype, new Order(), null as DataContractSerializer);
            agent = "author";
        }

        public FulfillmentBuilder WithId(Guid value)
        {
            id = value;
            return this;
        }

        public FulfillmentBuilder WithCreatedDateTime(DateTimeOffset value)
        {
            createdDateTime = value;
            return this;
        }

        public FulfillmentBuilder WithBaseUri(Uri value)
        {
            baseUri = value;
            return this;
        }

        public FulfillmentBuilder WithOrder(Order value)
        {
            order = new XmlSyndicationContent(MediaTypes.Restbucks.TypeAndSubtype, value, null as DataContractSerializer);
            return this;
        }

        public FulfillmentBuilder WithAgent(string value)
        {
            agent = value;
            return this;
        }

        public Fulfillment Build()
        {
            return new Fulfillment(id, createdDateTime, order, baseUri, agent);
        }
    }
}
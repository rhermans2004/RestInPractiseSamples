using Restbucks.OrderFulfillment.Model;
using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Tests.OrderFulfillment.Utility
{
    public class ETaggedEntityBuilder
    {
        private EntityTag entityTag;
        private Fulfillment fulfillment;

        public ETaggedEntityBuilder()
        {
            entityTag = null;
            fulfillment = FulfillmentBuilder.NewFulfillment();
        }

        public ETaggedEntityBuilder WithEntityTag(EntityTag value)
        {
            entityTag = value;
            return this;
        }

        public ETaggedEntityBuilder WithFulfillment(Fulfillment value)
        {
            fulfillment = value;
            return this;
        }

        public ETaggedEntity Build()
        {
            return new ETaggedEntity(entityTag, fulfillment);
        }
    }
}
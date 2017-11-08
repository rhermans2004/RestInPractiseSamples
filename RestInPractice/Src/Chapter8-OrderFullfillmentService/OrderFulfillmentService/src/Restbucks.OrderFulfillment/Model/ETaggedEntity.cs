using Restbucks.WcfRestToolkit.Http.HeaderValues;

namespace Restbucks.OrderFulfillment.Model
{
    public class ETaggedEntity
    {
        private readonly EntityTag entityTag;
        private readonly Fulfillment fulfillment;

        public ETaggedEntity(EntityTag entityTag, Fulfillment fulfillment)
        {
            this.entityTag = entityTag;
            this.fulfillment = fulfillment;
        }

        public EntityTag EntityTag
        {
            get { return entityTag; }
        }

        public Fulfillment Fulfillment
        {
            get { return fulfillment; }
        }
    }
}